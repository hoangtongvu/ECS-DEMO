using Unity.Entities;
using Components;
using Unity.Transforms;
using Components.ComponentMap;
using Utilities.Helpers;
using Core.UI.EntitySpawningPanel;
using Components.GameEntity.EntitySpawning;
using Components.Misc;

namespace Systems.Simulation.GameEntity.EntitySpawning
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class DespawnEntitySpawningPanelSystem : SystemBase
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
        }

        protected override void OnUpdate()
        {
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();
            var uiPrefabAndPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPrefabAndPoolMap>();

            foreach (var (spawningProfiles, uiSpawnedRef) in
                SystemAPI.Query<
                    DynamicBuffer<EntitySpawningProfileElement>
                    , RefRW<UISpawned>>()
                    .WithDisabled<WithinPlayerAutoInteractRadiusTag>())
            {
                bool canDespawn = uiSpawnedRef.ValueRO.IsSpawned;
                if (!canDespawn) continue;

                spawnedUIMap.Value.TryGetValue(uiSpawnedRef.ValueRO.UIID.Value, out var spawningPanel);

                this.DespawnProfileDisplays(uiPrefabAndPoolMap, spawnedUIMap, ((EntitySpawningPanelCtrl)spawningPanel).SpawningDisplaysHolder);

                this.DespawnEntitySpawningPanel(uiPrefabAndPoolMap, spawnedUIMap, ref uiSpawnedRef.ValueRW);

                uiSpawnedRef.ValueRW.IsSpawned = false;

            }

        }

        private void DespawnProfileDisplays(
            UIPrefabAndPoolMap uiPrefabAndPoolMap
            , SpawnedUIMap spawnedUIMap
            , SpawningDisplaysHolder spawningDisplaysHolder)
        {
            int length = spawningDisplaysHolder.SpawningProfileDisplayCtrls.Count;

            for (int i = 0; i < length; i++)
            {
                var runtimeUIID = spawningDisplaysHolder.SpawningProfileDisplayCtrls[i].RuntimeUIID;
                UISpawningHelper.Despawn(uiPrefabAndPoolMap, spawnedUIMap, runtimeUIID);
            }
            
            spawningDisplaysHolder.SpawningProfileDisplayCtrls.Clear();

        }

        private void DespawnEntitySpawningPanel(
            UIPrefabAndPoolMap uiPrefabAndPoolMap
            , SpawnedUIMap spawnedUIMap
            , ref UISpawned uiSpawned)
        {
            UISpawningHelper.Despawn(uiPrefabAndPoolMap, spawnedUIMap, uiSpawned.UIID.Value);
            uiSpawned.UIID = null;
        }

    }

}