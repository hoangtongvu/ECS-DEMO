using Components.GameEntity.Movement.MoveCommand;
using Components.Misc.WorldMap.PathFinding;
using Systems.Simulation.Misc.WorldMap.PathFinding;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Simulation.Unit.Movement
{
    [UpdateInGroup(typeof(PathFindingSystemGroup))]
    [UpdateBefore(typeof(HPAPathFindingSystem))]
    [BurstCompile]
    public partial struct SetTargetPosForPathFindingSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    MoveCommandElement
                    , TargetPosForPathFinding
                    , CanFindPathTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new SetTargetPosJob().ScheduleParallel(state.Dependency);
        }

        [WithAll(typeof(CanFindPathTag))]
        [BurstCompile]
        public partial struct SetTargetPosJob : IJobEntity
        {
            void Execute(
                in MoveCommandElement moveCommandElement
                , ref TargetPosForPathFinding targetPosForPathFinding)
            {
                targetPosForPathFinding.Value = moveCommandElement.Float3;
            }
        }

    }

}