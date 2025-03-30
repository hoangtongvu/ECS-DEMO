using Components.GameEntity.EntitySpawning.SpawningProfiles;
using Components.GameEntity.EntitySpawning.SpawningProfiles.Containers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.GameEntity.EntitySpawning
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

            su.AddOrSetComponentData(new EntityToContainerIndexMap
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