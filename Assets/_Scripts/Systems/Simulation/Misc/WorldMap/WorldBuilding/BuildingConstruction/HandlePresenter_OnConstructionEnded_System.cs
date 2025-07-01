using Components.Misc.Presenter;
using Components.Misc.Presenter.PresenterPrefabGO;
using Components.Misc.WorldMap.WorldBuilding;
using Core.Misc.Presenter;
using Unity.Entities;
using UnityEngine.Rendering;

namespace Systems.Simulation.Misc.WorldMap.WorldBuilding.BuildingConstruction
{
    [UpdateInGroup(typeof(EndConstructionProcessSystemGroup))]
    public partial class HandlePresenter_OnConstructionEnded_System : SystemBase
    {
        private EntityQuery query0;

        protected override void OnCreate()
        {
            this.query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    PresenterHolder
                    , PresenterOriginalMaterialHolder
                    , ConstructionNewlyEndedTag>()
                .Build();

            this.RequireForUpdate(this.query0);
        }

        protected override void OnUpdate()
        {
            foreach (var (presenterHolderRef, originalMaterialHolder, entity) in SystemAPI
                .Query<
                    RefRO<PresenterHolder>
                    , PresenterOriginalMaterialHolder>()
                .WithAll<ConstructionNewlyEndedTag>()
                .WithEntityAccess())
            {
                var basePresenter = presenterHolderRef.ValueRO.Value.Value;
                this.HandleBasePresenter(basePresenter, in originalMaterialHolder);
            }

        }

        private void HandleBasePresenter(
            BasePresenter basePresenter
            , in PresenterOriginalMaterialHolder presenterOriginalMaterialHolder)
        {
            var meshRenderer = basePresenter.MeshRenderer;

            meshRenderer.sharedMaterial = presenterOriginalMaterialHolder.Value.Value;
            meshRenderer.shadowCastingMode = ShadowCastingMode.On;
            meshRenderer.receiveShadows = true;
        }

    }

}