using Components.GameBuilding;
using Components.GameEntity.EntitySpawning;
using Components.Misc.WorldMap.WorldBuilding.BuildingConstruction;
using Components.Player;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Initialization.Misc.WorldMap.WorldBuilding.BuildingConstruction
{
    [UpdateInGroup(typeof(NewlySpawnedTagProcessSystemGroup))]
    [BurstCompile]
    public partial struct MarkPlayerBuildingAsBlueprintSystem : ISystem
    {
        private EntityQuery playerQuery;
        private EntityQuery gameBuildingQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.playerQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerTag>()
                .Build();

            this.gameBuildingQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    NewlySpawnedTag
                    , SpawnerEntityHolder>()
                .WithAll<
                    GameBuildingTag>()
                .Build();

            state.RequireForUpdate(this.gameBuildingQuery);
            state.RequireForUpdate(this.gameBuildingQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var buildingEntities = this.gameBuildingQuery.ToEntityArray(Allocator.Temp);
            int length = buildingEntities.Length;

            if (length == 0) return;

            var em = state.EntityManager;
            var playerEntity = this.playerQuery.GetSingletonEntity();
            var spawnerEntityHolders = this.gameBuildingQuery.ToComponentDataArray<SpawnerEntityHolder>(Allocator.Temp);

            for (int i = 0; i < length; i++)
            {
                var buildingEntity = buildingEntities[i];
                var spawnerEntity = spawnerEntityHolders[i].Value;

                if (spawnerEntity != playerEntity) continue;

                em.AddComponent<NeedChangeToBlueprintTag>(buildingEntity);
            }

        }

    }

}