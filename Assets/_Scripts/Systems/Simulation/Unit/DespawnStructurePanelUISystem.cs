using Unity.Entities;
using Components;
using Components.Unit.UnitSpawning;
using Components.Unit;
using Unity.Transforms;
using Components.ComponentMap;
using Utilities.Helpers;

namespace Systems.Simulation.Unit
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class DespawnStructurePanelUISystem : SystemBase
    {

        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitSelected
                    , UnitSpawningProfileElement
                    , UISpawned
                    , LocalTransform>()
                .Build();

            this.RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();
            var uiPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPoolMap>();

            foreach (var (selectedRef, spawningProfiles, uiSpawnedRef) in
                SystemAPI.Query<
                    RefRO<UnitSelected>
                    , DynamicBuffer<UnitSpawningProfileElement>
                    , RefRW<UISpawned>>())
            {

                if (this.CanDespawn(selectedRef, uiSpawnedRef))
                {
                    this.DespawnUnitProfileUI(uiPoolMap, spawnedUIMap, spawningProfiles);

                    this.DespawnMainPanel(uiPoolMap, spawnedUIMap, ref uiSpawnedRef.ValueRW);

                    // TODO: Use event to despawn is much more easier cause we don't need any return.
                    uiSpawnedRef.ValueRW.IsSpawned = false;
                }
            }

        }

        private bool CanDespawn(
            RefRO<UnitSelected> selectedRef
            , RefRW<UISpawned> uiSpawnedRef) => !selectedRef.ValueRO.Value && uiSpawnedRef.ValueRO.IsSpawned;


        private void DespawnUnitProfileUI(
            UIPoolMap uiPoolMap
            , SpawnedUIMap spawnedUIMap
            , DynamicBuffer<UnitSpawningProfileElement> spawningProfiles)
        {
            for (int i = 0; i < spawningProfiles.Length; i++)
            {
                ref var profile = ref spawningProfiles.ElementAt(i);
                UISpawningHelper.Despawn(uiPoolMap, spawnedUIMap, profile.UIID.Value);

                profile.UIID = null;
            }
        }

        private void DespawnMainPanel(
            UIPoolMap uiPoolMap
            , SpawnedUIMap spawnedUIMap
            , ref UISpawned uiSpawned)
        {
            UISpawningHelper.Despawn(uiPoolMap, spawnedUIMap, uiSpawned.UIID.Value);
            uiSpawned.UIID = null;
        }

    }
}