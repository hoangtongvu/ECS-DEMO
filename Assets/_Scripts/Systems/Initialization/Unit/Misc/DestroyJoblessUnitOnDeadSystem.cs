using Components.GameEntity.Damage;
using Components.GameEntity.Misc;
using Components.GameEntity.Misc.EntityCleanup;
using Components.Unit;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Utilities.Extensions;

namespace Systems.Initialization.Unit.Misc
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct DestroyJoblessUnitOnDeadSystem : ISystem
    {
        private EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    JoblessUnitTag>()
                .WithDisabled<
                    IsAliveTag>()
                .Build();

            state.RequireForUpdate(this.query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.query.ToEntityArray(Allocator.Temp);
            if (entities.Length == 0) return;

            var em = state.EntityManager;

            em.AddComponent<NeedDestroyBasePresenterTag>(entities);
            em.AddComponent<NeedDestroyEntityTag>(entities);
            em.AddComponent<PendingCleanupEntityTag>(entities);
            em.AddComponentData(entities, new PendingCleanupEntityTimer
            {
                TimeStamp = SystemAPI.Time.ElapsedTime,
                DurationSeconds = new(2f),
            });

        }

    }

}