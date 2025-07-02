using Components.GameEntity;
using Components.GameEntity.EntitySpawning.SpawningProfiles;
using Components.GameEntity.EntitySpawning.SpawningProfiles.Containers;
using Components.Misc.Presenter;
using Components.Misc.Presenter.PresenterPrefabGO;
using Components.Misc.WorldMap.WorldBuilding.BuildingConstruction;
using Core.Misc.Presenter;
using Systems.Initialization.Misc.Presenter;
using Systems.Initialization.Misc.Presenter.PresenterPrefabGO;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine.Rendering;

namespace Systems.Initialization.Misc.WorldMap.WorldBuilding.BuildingConstruction
{
    [UpdateInGroup(typeof(NeedSpawnPresenterTagProcessSystemGroup))]
    [UpdateAfter(typeof(SpawnPresenterGOsSystem))]
    [UpdateAfter(typeof(PresenterOriginalMaterialHolderInitSystem))]
    public partial class ChangeBuildingToBlueprintSystem : SystemBase
    {
        private EntityQuery query0;
        private EntityQuery query1;

        protected override void OnCreate()
        {
            this.query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    PresenterHolder
                    , PrimaryPrefabEntityHolder
                    , PresenterOriginalMaterialHolder
                    , NeedChangeToBlueprintTag>()
                .Build();

            this.query1 = SystemAPI.QueryBuilder()
                .WithAll<
                    BlueprintMaterialHolder
                    , EntityToContainerIndexMap
                    , EntitySpawningDurationsContainer>()
                .Build();

            this.RequireForUpdate(this.query0);
            this.RequireForUpdate(this.query1);

        }

        protected override void OnUpdate()
        {
            var blueprintMaterialHolder = this.query1.GetSingleton<BlueprintMaterialHolder>();
            var entityToContainerIndexMap = this.query1.GetSingleton<EntityToContainerIndexMap>().Value;
            var spawningDurationsContainer = this.query1.GetSingleton<EntitySpawningDurationsContainer>().Value;

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (presenterHolderRef, primaryPrefabEntityHolderRef, entity) in SystemAPI
                .Query<
                    RefRO<PresenterHolder>
                    , RefRO<PrimaryPrefabEntityHolder>>()
                .WithAll<
                    PresenterOriginalMaterialHolder
                    , NeedChangeToBlueprintTag>()
                .WithEntityAccess())
            {
                ecb.RemoveComponent<NeedChangeToBlueprintTag>(entity);

                float duration = spawningDurationsContainer[entityToContainerIndexMap[primaryPrefabEntityHolderRef.ValueRO]];
                if (duration == 0) continue;

                ecb.AddComponent(entity, new ConstructionRemaining
                {
                    Value = (uint)math.round(duration * 100),
                });

                var basePresenter = presenterHolderRef.ValueRO.Value.Value;
                this.HandleBasePresenter(basePresenter, in blueprintMaterialHolder);
            }

            ecb.Playback(this.EntityManager);

        }

        private void HandleBasePresenter(
            BasePresenter basePresenter
            , in BlueprintMaterialHolder blueprintMaterialHolder)
        {
            var meshRenderer = basePresenter.MeshRenderer;

            meshRenderer.sharedMaterial = blueprintMaterialHolder.Value.Value;
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;
        }

    }

}