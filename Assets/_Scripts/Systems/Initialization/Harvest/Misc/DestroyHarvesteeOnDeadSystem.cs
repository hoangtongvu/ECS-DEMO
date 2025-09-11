using Components.GameEntity.Damage;
using Components.GameEntity.Misc;
using Components.GameEntity.Misc.EntityCleanup;
using Components.Harvest;
using Systems.Initialization.GameEntity.Damage.DeadResolve;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Utilities.Extensions;

namespace Systems.Initialization.Harvest.Misc
{
    [UpdateInGroup(typeof(DeadResolveSystemGroup))]
    [BurstCompile]
    public partial struct DestroyHarvesteeOnDeadSystem : ISystem
    {
        private EntityQuery entityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    HarvesteeTag>()
                .WithAll<
                    IsDead>()
                .Build();

            state.RequireForUpdate(entityQuery);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = entityQuery.ToEntityArray(Allocator.Temp);
            if (entities.Length == 0) return;

            var em = state.EntityManager;

            em.AddComponent<NeedDestroyEntityTag>(entities);
            em.AddComponent<PendingCleanupEntityTag>(entities);
            em.AddComponentData(entities, new PendingCleanupEntityTimer
            {
                TimeStamp = SystemAPI.Time.ElapsedTime,
                DurationSeconds = new(0f),
            });
        }

    }

}