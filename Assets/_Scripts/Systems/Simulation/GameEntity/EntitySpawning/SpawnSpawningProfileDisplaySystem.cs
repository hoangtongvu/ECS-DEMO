using Unity.Entities;
using Components;
using Unity.Transforms;
using Unity.Mathematics;
using Core.UI.Identification;
using Core.UI.EntitySpawningPanel;
using Core.UI.EntitySpawningPanel.SpawningProfileDisplay;
using Components.ComponentMap;
using Utilities.Helpers;
using Components.Unit.UnitSelection;
using Components.GameEntity.EntitySpawning;
using Components.GameEntity.EntitySpawning.SpawningProfiles;
using Components.GameEntity.EntitySpawning.SpawningProfiles.Containers;
using System.Collections.Generic;

namespace Systems.Simulation.GameEntity.EntitySpawning
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class SpawnEntitySpawningPanelSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitSelectedTag
                    , EntitySpawningProfileElement
                    , UISpawned
                    , LocalTransform>()
                .Build();

            this.RequireForUpdate(query);
            this.RequireForUpdate<SpawnedUIMap>();
            this.RequireForUpdate<UIPrefabAndPoolMap>();
            this.RequireForUpdate<EntityToContainerIndexMap>();
            this.RequireForUpdate<EntitySpawningSpritesContainer>();
        }

        protected override void OnUpdate()
        {
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();
            var uiPrefabAndPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPrefabAndPoolMap>();
            var entityToContainerIndexMap = SystemAPI.GetSingleton<EntityToContainerIndexMap>();
            var spritesContainer = SystemAPI.GetSingleton<EntitySpawningSpritesContainer>();

            foreach (var (spawningProfiles, uiSpawnedRef, transformRef, spawnerEntity) in
                SystemAPI.Query<
                    DynamicBuffer<EntitySpawningProfileElement>
                    , RefRW<UISpawned>
                    , RefRO<LocalTransform>>()
                    .WithAll<UnitSelectedTag>()
                    .WithEntityAccess())
            {
                bool canSpawn = !uiSpawnedRef.ValueRO.IsSpawned;
                if (!canSpawn) continue;

                float3 spawnPos = transformRef.ValueRO.Position + uiSpawnedRef.ValueRO.SpawnPosOffset;

                this.SpawnEntitySpawningPanel(
                    uiPrefabAndPoolMap
                    , spawnedUIMap
                    , spawnPos
                    , ref uiSpawnedRef.ValueRW
                    , out var entitySpawningPanelCtrl);

                this.SpawnProfileDisplays(
                    uiPrefabAndPoolMap
                    , spawnedUIMap
                    , in entityToContainerIndexMap
                    , in spritesContainer
                    , spawningProfiles
                    , spawnPos
                    , entitySpawningPanelCtrl
                    , in spawnerEntity);

                uiSpawnedRef.ValueRW.IsSpawned = true;

            }

        }

        private void SpawnEntitySpawningPanel(
            UIPrefabAndPoolMap uiPrefabAndPoolMap
            , SpawnedUIMap spawnedUIMap
            , float3 spawnPos
            , ref UISpawned uiSpawned
            , out EntitySpawningPanelCtrl entitySpawningPanelCtrl)
        {
            entitySpawningPanelCtrl =
                (EntitySpawningPanelCtrl) UISpawningHelper.Spawn(
                    uiPrefabAndPoolMap
                    , spawnedUIMap
                    , UIType.EntitySpawningPanel
                    , spawnPos);

            uiSpawned.UIID = entitySpawningPanelCtrl.RuntimeUIID;

            entitySpawningPanelCtrl.gameObject.SetActive(true);
        }

        private void SpawnProfileDisplays(
            UIPrefabAndPoolMap uiPrefabAndPoolMap
            , SpawnedUIMap spawnedUIMap
            , in EntityToContainerIndexMap entityToContainerIndexMap
            , in EntitySpawningSpritesContainer spritesContainer
            , DynamicBuffer<EntitySpawningProfileElement> spawningProfiles
            , float3 spawnPos
            , EntitySpawningPanelCtrl entitySpawningPanelCtrl
            , in Entity spawnerEntity)
        {
            for (int i = 0; i < spawningProfiles.Length; i++)
            {
                ref var profile = ref spawningProfiles.ElementAt(i);

                var keyEntity = profile.PrefabToSpawn;

                if (!entityToContainerIndexMap.Value.TryGetValue(keyEntity, out int containerIndex))
                    throw new KeyNotFoundException($"{nameof(EntityToContainerIndexMap)} does not contain key: {keyEntity}");

                // Grid layout won't config Z dimension, that why setting unitProfileUICtrl position is required.
                var profileDisplayCtrl =
                    (SpawningProfileDisplayCtrl) UISpawningHelper.Spawn(
                        uiPrefabAndPoolMap
                        , spawnedUIMap
                        , UIType.SpawningProfileDisplay
                        , spawnPos);

                profileDisplayCtrl.ProgressBar.ClearProgress();
                profileDisplayCtrl.SpawnCountText.SetSpawnCount(profile.SpawnCount.Value);

                profileDisplayCtrl.SpawnerEntity = spawnerEntity;
                profileDisplayCtrl.SpawningProfileElementIndex = i;

                profileDisplayCtrl.ProfilePic.sprite = spritesContainer.Value[containerIndex];
                entitySpawningPanelCtrl.SpawningDisplaysHolder.Add(profileDisplayCtrl);

                profileDisplayCtrl.gameObject.SetActive(true);

            }

        }

    }

}