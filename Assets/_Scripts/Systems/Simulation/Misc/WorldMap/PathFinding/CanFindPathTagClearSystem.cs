using Components.Misc.WorldMap.PathFinding;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Simulation.Misc.WorldMap.PathFinding
{
    [UpdateInGroup(typeof(PathFindingSystemGroup), OrderLast = true)]
    [BurstCompile]
    public partial struct CanFindPathTagClearSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CanFindPathTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new TagClearJob().ScheduleParallel(state.Dependency);
        }

        [BurstCompile]
        private partial struct TagClearJob : IJobEntity
        {
            void Execute(EnabledRefRW<CanFindPathTag> canFindPathTag)
            {
                canFindPathTag.ValueRW = false;
            }
        }

    }

}