using Components.Misc.WorldMap.WorldBuilding.PlacementPreview;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.Misc.WorldMap.WorldBuilding
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct PlacementPreviewSpriteInitSystem : ISystem
    {
        private EntityQuery prefabEntityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.prefabEntityQuery = SystemAPI.QueryBuilder()
                .WithAll<PlacementPreviewSpriteTag>()
                .WithOptions(EntityQueryOptions.IncludePrefab)
                .Build();

            state.RequireForUpdate(prefabEntityQuery);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var prefabEntity = prefabEntityQuery.GetSingletonEntity();
            state.EntityManager.Instantiate(prefabEntity);

        }

    }

}