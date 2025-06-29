using Components.Misc.Presenter;
using Components.Misc.Presenter.PresenterPrefabGO;
using Components.Misc.WorldMap.WorldBuilding.BuildingConstruction;
using Core.Misc.Presenter;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Rendering;

namespace Systems.Simulation.Misc.WorldMap.WorldBuilding.BuildingConstruction
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class EndConstructionProcessSystem : SystemBase
    {
        private EntityQuery query0;

        protected override void OnCreate()
        {
            this.query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    ConstructionRemaining
                    , PresenterHolder
                    , PresenterOriginalMaterialHolder>()
                .Build();

            this.RequireForUpdate(this.query0);
        }

        protected override void OnUpdate()
        {
            var entities = new NativeList<Entity>(10, Allocator.Temp);

            foreach (var (constructionRemainingRef, presenterHolderRef, originalMaterialHolder, entity) in SystemAPI
                .Query<
                    RefRO<ConstructionRemaining>
                    , RefRO<PresenterHolder>
                    , PresenterOriginalMaterialHolder>()
                .WithEntityAccess())
            {
                bool constructionEnded = constructionRemainingRef.ValueRO.Value == uint.MinValue;
                if (!constructionEnded) continue;

                var basePresenter = presenterHolderRef.ValueRO.Value.Value;
                this.HandleBasePresenter(basePresenter, in originalMaterialHolder);

                entities.Add(entity);
            }

            this.EntityManager.RemoveComponent<ConstructionRemaining>(entities.AsArray());

        }

        private void HandleBasePresenter(
            BasePresenter basePresenter
            , in PresenterOriginalMaterialHolder presenterOriginalMaterialHolder)
        {
            var meshRenderer = basePresenter.GetComponent<MeshRenderer>();

            meshRenderer.sharedMaterial = presenterOriginalMaterialHolder.Value.Value;
            meshRenderer.shadowCastingMode = ShadowCastingMode.On;
            meshRenderer.receiveShadows = true;
        }

    }

}