using Components.MyEntity.EntitySpawning.GlobalCostMap;
using Components.MyEntity.EntitySpawning.GlobalCostMap.Containers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.MyEntity.EntitySpawning
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct SpawningProfileComponentsInitSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var su = SingletonUtilities.GetInstance(state.EntityManager);

            su.AddOrSetComponentData(new LatestCostMapIndex
            {
                Value = -1,
            });

            su.AddOrSetComponentData(new EntityToCostMapIndexMap
            {
                Value = new(20, Allocator.Persistent),
            });

            su.AddOrSetComponentData(new EntitySpawningCostsContainer
            {
                Value = new(60, Allocator.Persistent),
            });

            su.AddOrSetComponentData(new EntitySpawningDurationsContainer
            {
                Value = new(20, Allocator.Persistent),
            });

            su.AddOrSetComponentData(new EntitySpawningSpritesContainer
            {
                Value = new(20, Allocator.Persistent),
            });

            state.Enabled = false;

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
        }

    }

}