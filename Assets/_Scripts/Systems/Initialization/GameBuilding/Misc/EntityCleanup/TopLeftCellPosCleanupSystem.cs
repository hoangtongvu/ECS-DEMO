using Components.GameEntity.Misc.EntityCleanup;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Systems.Initialization.GameEntity.Misc.EntityCleanup;
using Components.Misc.WorldMap.Misc;

namespace Systems.Initialization.GameBuilding.Misc.EntityCleanup
{
    [UpdateInGroup(typeof(CleanupEntityHandleSystemGroup), OrderLast = true)]
    [UpdateBefore(typeof(NeedCleanupEntityTagRemoveSystem))]
    [BurstCompile]
    public partial struct TopLeftCellPosCleanupSystem : ISystem
    {
        private EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    NeedCleanupEntityTag
                    , TopLeftCellPos>()
                .Build();

            state.RequireForUpdate(this.query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.query.ToEntityArray(Allocator.Temp);
            int length = entities.Length;

            if (length == 0) return;

            state.EntityManager.RemoveComponent<TopLeftCellPos>(entities);
        }

    }

}