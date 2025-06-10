using Components.GameEntity.Damage;
using Components.Harvest;
using Components.Misc.WorldMap;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Initialization.Harvest
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct DestroyHarvesteeOnDeadSystem : ISystem
    {
        private EntityQuery entityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    HarvesteeTag>()
                .WithDisabled<IsAliveTag>()
                .Build();

            state.RequireForUpdate(this.entityQuery);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var toDestroyEntities = this.entityQuery.ToEntityArray(Allocator.Temp);

            if (toDestroyEntities.Length == 0) return;

            SystemAPI.GetSingletonRW<WorldMapChangedTag>().ValueRW.Value = true;

            state.EntityManager.DestroyEntity(toDestroyEntities);

        }

    }

}