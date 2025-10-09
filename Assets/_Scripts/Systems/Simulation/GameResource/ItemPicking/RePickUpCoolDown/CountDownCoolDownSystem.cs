using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Components.GameResource.ItemPicking.Pickee.RePickUpCoolDown;

namespace Systems.Simulation.GameResource.ItemPicking.RePickUpCoolDown
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct CountDownCoolDownSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    IsInRePickUpCoolDown
                    , PreviousPickerEntity
                    , PreviousPickerPickupCoolDownSeconds>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new CountDownJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
            }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct CountDownJob : IJobEntity
        {
            [ReadOnly] public float DeltaTime;

            [BurstCompile]
            void Execute(
                EnabledRefRW<IsInRePickUpCoolDown> isInRePickUpCoolDownTag
                , ref PreviousPickerEntity previousPickerEntity
                , ref PreviousPickerPickupCoolDownSeconds coolDownSeconds)
            {
                coolDownSeconds -= this.DeltaTime;
                if (coolDownSeconds > 0) return;

                coolDownSeconds = 0;
                isInRePickUpCoolDownTag.ValueRW = false;
                previousPickerEntity.Value = Entity.Null;
            }
        }

    }

}