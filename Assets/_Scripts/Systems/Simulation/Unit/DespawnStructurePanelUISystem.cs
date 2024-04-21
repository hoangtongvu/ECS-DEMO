using Unity.Entities;
using Components;
using Components.Unit.UnitSpawning;
using Components.Unit;
using Unity.Transforms;
using Core.Spawner;
using Components.ComponentMap;

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

            foreach (var (selectedRef, spawningProfiles, uiSpawnedRef) in
                SystemAPI.Query<
                    RefRO<UnitSelected>
                    , DynamicBuffer<UnitSpawningProfileElement>
                    , RefRW<UISpawned>>())
            {

                if (this.CanDespawn(selectedRef, uiSpawnedRef))
                {
                    this.DespawnUnitProfileUI(spawningProfiles);

                    this.DespawnMainPanel(ref uiSpawnedRef.ValueRW);

                    // TODO: Use event to despawn is much more easier cause we don't need any return.
                    uiSpawnedRef.ValueRW.IsSpawned = false;
                }
            }

        }

        private bool CanDespawn(
            RefRO<UnitSelected> selectedRef
            , RefRW<UISpawned> uiSpawnedRef) => !selectedRef.ValueRO.Value && uiSpawnedRef.ValueRO.IsSpawned;


        private void DespawnUnitProfileUI(DynamicBuffer<UnitSpawningProfileElement> spawningProfiles)
        {
            for (int i = 0; i < spawningProfiles.Length; i++)
            {
                ref var profile = ref spawningProfiles.ElementAt(i);
                UISpawner.Instance.Despawn(profile.UIID.Value);

                profile.UIID = null;
            }
        }

        private void DespawnMainPanel(ref UISpawned uiSpawned)
        {
            UISpawner.Instance.Despawn(uiSpawned.UIID.Value);
            uiSpawned.UIID = null;
        }

    }
}