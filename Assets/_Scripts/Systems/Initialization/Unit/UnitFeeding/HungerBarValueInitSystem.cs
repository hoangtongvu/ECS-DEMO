using Components.GameEntity.EntitySpawning;
using Components.Unit.Misc;
using Components.Unit.UnitFeeding;
using Core.Unit.UnitFeeding;
using Systems.Initialization.Misc;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Utilities.Extensions;

namespace Systems.Initialization.Unit.UnitFeeding
{
    [UpdateInGroup(typeof(NewlySpawnedTagProcessSystemGroup))]
    [BurstCompile]
    public partial struct HungerBarValueInitSystem : ISystem
    {
        private EntityQuery query0;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    NewlySpawnedTag>()
                .WithAll<
                    UnitTag>()
                .WithAllRW<
                    HungerBarValue>()
                .Build();

            state.RequireForUpdate(this.query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.query0.ToEntityArray(Allocator.Temp);
            int length = entities.Length;

            if (length == 0) return;
            var em = state.EntityManager;

            em.SetComponentData(entities, new HungerBarValue(UnitFeedingConfigConstants.UnitFeedingConfigs.HungerBarConfigs.HungerBarCap));
        }

    }

}