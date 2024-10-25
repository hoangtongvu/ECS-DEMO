using Components.Unit;
using Unity.Burst;
using Unity.Entities;
using Utilities;


namespace Systems.Initialization.Unit
{

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct SelectedUnitMarkerPrefabInitSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // We don't Add prefab entity in OnCreate() due to async sub scene loading.
            // Create duplicated prefabEntityQuery because the system only run once, caching the variable of query is redundant.

            var prefabEntityQuery = SystemAPI.QueryBuilder()
                .WithAll<SelectedUnitMarkerTag>()
                .WithOptions(EntityQueryOptions.IncludePrefab)
                .Build();

            state.RequireForUpdate(prefabEntityQuery);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var prefabEntityQuery = SystemAPI.QueryBuilder()
                .WithAll<SelectedUnitMarkerTag>()
                .WithOptions(EntityQueryOptions.IncludePrefab)
                .Build();

            var prefabEntity = prefabEntityQuery.GetSingletonEntity();

            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new SelectedUnitMarkerPrefab
                {
                    Value = prefabEntity,
                });

        }


    }
}