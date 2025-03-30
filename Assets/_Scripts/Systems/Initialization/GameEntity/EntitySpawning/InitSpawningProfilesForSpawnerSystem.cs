using Components.GameEntity;
using Components.MyEntity.EntitySpawning;
using Components.MyEntity.EntitySpawning.GlobalCostMap;
using Components.MyEntity.EntitySpawning.GlobalCostMap.Containers;
using Components.Tool;
using System.Collections.Generic;
using Systems.Initialization.Misc.WorldMap;
using Unity.Entities;

namespace Systems.Initialization.GameEntity.EntitySpawning
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(AddSpawningProfilesSystemGroup))]
    [UpdateAfter(typeof(MapChangedSystemGroup))] // Make sure update after entity spawned to prevent missing NewlySpawnedTag.
    public partial struct InitSpawningProfilesForSpawnersSystem : ISystem
    {
        private EntityQuery query;

        public void OnCreate(ref SystemState state)
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    ToolProfilesSOHolder
                    , AfterBakedPrefabsElement>()
                .Build();

            state.RequireForUpdate(this.query);
            state.RequireForUpdate<NewlySpawnedTag>();

        }

        public void OnUpdate(ref SystemState state)
        {
            var entityToContainerIndexMap = SystemAPI.GetSingleton<EntityToCostMapIndexMap>();
            var durationsContainer = SystemAPI.GetSingleton<EntitySpawningDurationsContainer>();
            var spritesContainer = SystemAPI.GetSingleton<EntitySpawningSpritesContainer>();

            foreach (var spawningProfileElements in
                SystemAPI.Query<
                    DynamicBuffer<EntitySpawningProfileElement>>()
                    .WithAll<NewlySpawnedTag>())
            {
                int count = spawningProfileElements.Length;

                for (int i = 0; i < count; i++)
                {
                    ref var spawningProfileElement = ref spawningProfileElements.ElementAt(i);
                    var key = spawningProfileElement.PrefabToSpawn;

                    if (!entityToContainerIndexMap.Value.TryGetValue(key, out int containerIndex))
                        throw new KeyNotFoundException($"{nameof(EntityToCostMapIndexMap)} does not contain key: {key}");

                    spawningProfileElement.UnitSprite = spritesContainer.Value[containerIndex];
                    spawningProfileElement.SpawnDuration = new()
                    {
                        DurationCounterSeconds = 0f,
                        SpawnDurationSeconds = durationsContainer.Value[containerIndex],
                    };

                }

            }

        }

    }

}