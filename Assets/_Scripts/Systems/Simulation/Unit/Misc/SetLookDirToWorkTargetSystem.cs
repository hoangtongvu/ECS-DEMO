using Components.GameEntity.Misc;
using Components.GameEntity.Movement.MoveCommand;
using Components.Unit.Misc;
using Components.Unit.Reaction;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Simulation.Unit.Misc
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct SetLookDirToWorkTargetSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    LocalTransform
                    , MoveCommandElement
                    , LookDirectionXZ
                    , InteractReaction.StartedTag>()
                .WithAll<
                    UnitTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new RotateJob().ScheduleParallel();
        }

        [WithAll(typeof(UnitTag))]
        [BurstCompile]
        private partial struct RotateJob : IJobEntity
        {
            void Execute(
                in LocalTransform transform
                , in MoveCommandElement moveCommandElement
                , ref LookDirectionXZ lookDirectionXZ)
            {
                float3 targetPosition = moveCommandElement.Float3;
                targetPosition.y = transform.Position.y;

                float3 direction = math.normalize(targetPosition - transform.Position);
                lookDirectionXZ.Value = new(direction.x, direction.z);
            }

        }

    }

}