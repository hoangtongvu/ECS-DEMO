using Unity.Entities;
using Components;
using Components.MyEntity.EntitySpawning;
using Unity.Transforms;
using Unity.Mathematics;
using Core.UI.Identification;
using Core.UI.EntitySpawningPanel;
using Core.UI.EntitySpawningPanel.SpawningProfileDisplay;
using Components.ComponentMap;
using Utilities.Helpers;
using Components.Unit.UnitSelection;

namespace Systems.Simulation.MyEntity.EntitySpawning
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
        }

        protected override void OnUpdate()
        {
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();
            var uiPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPoolMap>();


            foreach (var (spawningProfiles, uiSpawnedRef, transformRef) in
                SystemAPI.Query<
                    DynamicBuffer<EntitySpawningProfileElement>
                    , RefRW<UISpawned>
                    , RefRO<LocalTransform>>()
                    .WithAll<UnitSelectedTag>())
            {
                bool canSpawn = !uiSpawnedRef.ValueRO.IsSpawned;
                if (!canSpawn) continue;

                float3 spawnPos = transformRef.ValueRO.Position + uiSpawnedRef.ValueRO.SpawnPosOffset;

                this.SpawnEntitySpawningPanel(
                    uiPoolMap
                    , spawnedUIMap
                    , spawnPos
                    , ref uiSpawnedRef.ValueRW
                    , out var entitySpawningPanelCtrl);

                this.SpawnProfileDisplays(
                    uiPoolMap
                    , spawnedUIMap
                    , spawningProfiles
                    , spawnPos
                    , entitySpawningPanelCtrl);

                uiSpawnedRef.ValueRW.IsSpawned = true;

            }

        }


        private void SpawnEntitySpawningPanel(
            UIPoolMap uiPoolMap
            , SpawnedUIMap spawnedUIMap
            , float3 spawnPos
            , ref UISpawned uiSpawned
            , out EntitySpawningPanelCtrl entitySpawningPanelCtrl)
        {

            entitySpawningPanelCtrl =
                (EntitySpawningPanelCtrl) UISpawningHelper.Spawn(
                    uiPoolMap
                    , spawnedUIMap
                    , UIType.EntitySpawningPanel
                    , spawnPos);

            uiSpawned.UIID = entitySpawningPanelCtrl.UIID;

            entitySpawningPanelCtrl.gameObject.SetActive(true);
        }

        private void SpawnProfileDisplays(
            UIPoolMap uiPoolMap
            , SpawnedUIMap spawnedUIMap
            , DynamicBuffer<EntitySpawningProfileElement> spawningProfiles
            , float3 spawnPos
            , EntitySpawningPanelCtrl entitySpawningPanelCtrl)
        {
            for (int i = 0; i < spawningProfiles.Length; i++)
            {
                ref var profile = ref spawningProfiles.ElementAt(i);

                // Grid layout won't config Z dimension, that why setting unitProfileUICtrl position is required.
                var profileDisplayCtrl =
                    (SpawningProfileDisplayCtrl) UISpawningHelper.Spawn(
                        uiPoolMap
                        , spawnedUIMap
                        , UIType.SpawningProfileDisplay
                        , spawnPos);

                profileDisplayCtrl.ProgressBar.ClearProgress();
                profileDisplayCtrl.SpawnCountText.SetSpawnCount(profile.SpawnCount.Value);

                profile.UIID = profileDisplayCtrl.UIID;

                profileDisplayCtrl.ProfilePic.sprite = profile.UnitSprite.Value;
                entitySpawningPanelCtrl.SpawningDisplaysHolder.Add(profileDisplayCtrl);

                profileDisplayCtrl.gameObject.SetActive(true);
            }
        }

    }
}