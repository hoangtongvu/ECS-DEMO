using Unity.Entities;
using Components;
using Components.MyEntity.EntitySpawning;
using Unity.Transforms;
using Components.ComponentMap;
using Utilities.Helpers;
using Components.Unit.UnitSelection;

namespace Systems.Simulation.MyEntity.EntitySpawning
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class DespawnEntitySpawningPanelSystem : SystemBase
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

            foreach (var (spawningProfiles, uiSpawnedRef) in
                SystemAPI.Query<
                    DynamicBuffer<EntitySpawningProfileElement>
                    , RefRW<UISpawned>>()
                    .WithDisabled<UnitSelectedTag>())
            {

                if (this.CanDespawn(uiSpawnedRef))
                {
                    this.DespawnProfileDisplays(uiPoolMap, spawnedUIMap, spawningProfiles);

                    this.DespawnEntitySpawningPanel(uiPoolMap, spawnedUIMap, ref uiSpawnedRef.ValueRW);

                    // TODO: Use event to despawn is much more easier cause we don't need any return.
                    uiSpawnedRef.ValueRW.IsSpawned = false;
                }
            }

        }

        private bool CanDespawn(
            RefRW<UISpawned> uiSpawnedRef) => uiSpawnedRef.ValueRO.IsSpawned;


        private void DespawnProfileDisplays(
            UIPoolMap uiPoolMap
            , SpawnedUIMap spawnedUIMap
            , DynamicBuffer<EntitySpawningProfileElement> spawningProfiles)
        {
            for (int i = 0; i < spawningProfiles.Length; i++)
            {
                ref var profile = ref spawningProfiles.ElementAt(i);
                UISpawningHelper.Despawn(uiPoolMap, spawnedUIMap, profile.UIID.Value);

                profile.UIID = null;
            }
        }

        private void DespawnEntitySpawningPanel(
            UIPoolMap uiPoolMap
            , SpawnedUIMap spawnedUIMap
            , ref UISpawned uiSpawned)
        {
            UISpawningHelper.Despawn(uiPoolMap, spawnedUIMap, uiSpawned.UIID.Value);
            uiSpawned.UIID = null;
        }

    }
}