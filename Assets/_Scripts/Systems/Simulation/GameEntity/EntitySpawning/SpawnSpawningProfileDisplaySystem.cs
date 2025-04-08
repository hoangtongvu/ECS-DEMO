using Unity.Entities;
using Components;
using Unity.Transforms;
using Unity.Mathematics;
using Core.UI.Identification;
using Core.UI.EntitySpawningPanel;
using Core.UI.EntitySpawningPanel.SpawningProfileDisplay;
using Components.ComponentMap;
using Utilities.Helpers;
using Components.GameEntity.EntitySpawning;
using Components.GameEntity.EntitySpawning.SpawningProfiles;
using Components.GameEntity.EntitySpawning.SpawningProfiles.Containers;
using System.Collections.Generic;
using Components.Misc;
using LitMotion;

namespace Systems.Simulation.GameEntity.EntitySpawning
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class SpawnEntitySpawningPanelSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    WithinPlayerAutoInteractRadiusTag
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
                    .WithAll<WithinPlayerAutoInteractRadiusTag>()
                    .WithEntityAccess())
            {
                bool canSpawn = !uiSpawnedRef.ValueRO.IsSpawned;
                if (!canSpawn) continue;

                float3 fromPos = transformRef.ValueRO.Position;
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
                    , in spawnerEntity
                    , out var spawningProfileDisplayCtrls);

                this.SetTween(entitySpawningPanelCtrl, spawningProfileDisplayCtrls, fromPos, spawnPos);

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
        }

        private void SpawnProfileDisplays(
            UIPrefabAndPoolMap uiPrefabAndPoolMap
            , SpawnedUIMap spawnedUIMap
            , in EntityToContainerIndexMap entityToContainerIndexMap
            , in EntitySpawningSpritesContainer spritesContainer
            , DynamicBuffer<EntitySpawningProfileElement> spawningProfiles
            , float3 spawnPos
            , EntitySpawningPanelCtrl entitySpawningPanelCtrl
            , in Entity spawnerEntity
            , out List<SpawningProfileDisplayCtrl> spawningProfileDisplayCtrls)
        {
            int count = spawningProfiles.Length;
            spawningProfileDisplayCtrls = new(count);

            for (int i = 0; i < count; i++)
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

                spawningProfileDisplayCtrls.Add(profileDisplayCtrl);
            }

        }

        private void SetTween(
            EntitySpawningPanelCtrl entitySpawningPanelCtrl
            , List<SpawningProfileDisplayCtrl> spawningProfileDisplayCtrls
            , float3 fromPos
            , float3 toPos)
        {
            int profileCount = spawningProfileDisplayCtrls.Count;

            entitySpawningPanelCtrl.gameObject.SetActive(true);
            LMotion.Create(float3.zero, new float3(1, 1, 1), 1f)
                .WithEase(Ease.OutExpo)
                .Bind(tempScale => entitySpawningPanelCtrl.transform.localScale = tempScale);

            LMotion.Create(fromPos, toPos, 1f)
                .WithEase(Ease.OutExpo)
                .Bind(tempPos => entitySpawningPanelCtrl.transform.position = tempPos);

            for (int i = 0; i < profileCount; i++)
            {
                var profileDisplayCtrl = spawningProfileDisplayCtrls[i];
                profileDisplayCtrl.gameObject.SetActive(true);

                LMotion.Create(float3.zero, new float3(1, 1, 1), 1f)
                    .WithEase(Ease.OutExpo)
                    .Bind(tempScale => profileDisplayCtrl.transform.localScale = tempScale);

            }

        }

    }

}