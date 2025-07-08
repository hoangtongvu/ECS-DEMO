using Components.GameEntity.Misc.EntityCleanup;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Initialization.GameEntity.Misc.EntityCleanup
{
    [UpdateInGroup(typeof(CleanupEntityHandleSystemGroup), OrderFirst = true)]
    [BurstCompile]
    public partial struct PendingCleanupEntityHandleSystem : ISystem
    {
        private EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    PendingCleanupEntityTag
                    , PendingCleanupEntityTimer>()
                .Build();

            state.RequireForUpdate(this.query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.query.ToEntityArray(Allocator.Temp);
            int length = entities.Length;

            if (length == 0) return;

            var pendingDestroyEntityTimers = this.query.ToComponentDataArray<PendingCleanupEntityTimer>(Allocator.Temp);

            for (int i = 0; i < length; i++)
            {
                var pendingTimer = pendingDestroyEntityTimers[i];
                double elapsedSeconds = SystemAPI.Time.ElapsedTime - pendingTimer.TimeStamp;

                if (elapsedSeconds < pendingTimer.DurationSeconds) continue;

                var entity = entities[i];

                state.EntityManager.RemoveComponent<PendingCleanupEntityTag>(entity);
                state.EntityManager.RemoveComponent<PendingCleanupEntityTimer>(entity);
                state.EntityManager.AddComponent<NeedCleanupEntityTag>(entity);
            }

        }

    }

}