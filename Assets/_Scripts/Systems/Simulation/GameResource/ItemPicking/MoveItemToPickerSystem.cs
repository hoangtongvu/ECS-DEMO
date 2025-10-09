using Components.GameResource.ItemPicking.Pickee;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Simulation.GameResource.ItemPicking
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct MoveItemToPickerSystem : ISystem
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
            new MoveJob
            {
                MoveSpeed = 3f,
                DeltaTime = SystemAPI.Time.DeltaTime,
            }.ScheduleParallel();
        }

        [WithAll(typeof(IsCandidateItem))]
        [BurstCompile]
        private partial struct MoveJob : IJobEntity
        {
            [ReadOnly] public float MoveSpeed;
            [ReadOnly] public float DeltaTime;

            void Execute(
                ref LocalTransform transform
                , in PickerPos pickerPos)
            {
                transform.Position = math.lerp(transform.Position, pickerPos.Value, this.MoveSpeed * this.DeltaTime);
            }
        }

    }

}