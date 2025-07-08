using Components.GameEntity.Misc.EntityCleanup;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Initialization.GameEntity.Misc.EntityCleanup
{
    [UpdateInGroup(typeof(CleanupEntityHandleSystemGroup), OrderLast = true)]
    [BurstCompile]
    public partial struct NeedCleanupEntityTagRemoveSystem : ISystem
    {
        private EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    NeedCleanupEntityTag>()
                .Build();

            state.RequireForUpdate(this.query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.query.ToEntityArray(Allocator.Temp);
            state.EntityManager.RemoveComponent<NeedCleanupEntityTag>(entities);
        }

    }

}