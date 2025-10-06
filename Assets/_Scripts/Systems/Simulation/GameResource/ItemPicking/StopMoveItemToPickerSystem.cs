using Components.GameResource.ItemPicking;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Simulation.GameResource.ItemPicking
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(MoveItemToPickerSystem))]
    [BurstCompile]
    public partial struct StopMoveItemToPickerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    LocalTransform
                    , PickerPos>()
                .WithAll<
                    IsCandidateItem>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            const float interactRadius = 3f;

            new StopMoveJob
            {
                CandidateItemRadius = interactRadius,
            }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct StopMoveJob : IJobEntity
        {
            [ReadOnly] public float CandidateItemRadius;

            void Execute(
                in LocalTransform transform
                , in PickerPos pickerPos
                , EnabledRefRW<IsCandidateItem> isCandidateItemTag)
            {
                float distance = math.distance(transform.Position, pickerPos.Value);
                if (distance <= this.CandidateItemRadius) return;

                isCandidateItemTag.ValueRW = false;
            }
        }

    }

}