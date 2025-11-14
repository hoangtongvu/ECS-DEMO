using Components.GameEntity.EntitySpawning;
using Components.GameEntity.Misc;
using Components.Unit.DarkUnit;
using Core.Unit.DarkUnit;
using Systems.Initialization.Misc;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Utilities.Extensions;

namespace Systems.Initialization.Unit.DarkUnit
{
    [UpdateInGroup(typeof(NewlySpawnedTagProcessSystemGroup))]
    [BurstCompile]
    public partial struct NewlySpawnedDarkUnitFactionSetSystem : ISystem
    {
        private EntityQuery darkUnitQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.darkUnitQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    DarkUnitTag
                    , FactionIndex
                    , NewlySpawnedTag>()
                .Build();

            state.RequireForUpdate(this.darkUnitQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.darkUnitQuery.ToEntityArray(Allocator.Temp);

            state.EntityManager.SetComponentData(entities, new FactionIndex
            {
                Value = DarkUnitConfigConstants.DefaultDarkUnitFactionIndex,
            });
        }

    }

}