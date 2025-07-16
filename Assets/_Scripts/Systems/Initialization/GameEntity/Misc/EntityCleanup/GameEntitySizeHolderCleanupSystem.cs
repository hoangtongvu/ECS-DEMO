using Components.GameEntity.Misc;
using Components.GameEntity.Misc.EntityCleanup;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Initialization.GameEntity.Misc.EntityCleanup
{
    [UpdateInGroup(typeof(CleanupEntityHandleSystemGroup), OrderLast = true)]
    [UpdateBefore(typeof(NeedCleanupEntityTagRemoveSystem))]
    [BurstCompile]
    public partial struct GameEntitySizeHolderCleanupSystem : ISystem
    {
        private EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    NeedCleanupEntityTag
                    , GameEntitySizeHolder>()
                .Build();

            state.RequireForUpdate(this.query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.query.ToEntityArray(Allocator.Temp);
            int length = entities.Length;

            if (length == 0) return;

            state.EntityManager.RemoveComponent<GameEntitySizeHolder>(entities);
        }

    }

}