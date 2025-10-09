using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Components.GameResource.ItemPicking.Pickee.RePickUpCoolDown;
using Systems.Initialization.Misc;
using Components.GameEntity.EntitySpawning;

namespace Systems.Initialization.GameResource.ItemPicking.RePickUpCoolDown
{
    [UpdateInGroup(typeof(NewlySpawnedTagProcessSystemGroup))]
    [BurstCompile]
    public partial struct SetRePickUpCoolDownSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    NewlySpawnedTag
                    , IsInRePickUpCoolDown
                    , PreviousPickerEntity
                    , PreviousPickerPickupCoolDownSeconds>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate<PreviousPickerPickupMaxCoolDownSeconds>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float maxCoolDownSeconds = SystemAPI.GetSingleton<PreviousPickerPickupMaxCoolDownSeconds>().value;

            new SetCoolDownJob
            {
                MaxCoolDownSeconds = maxCoolDownSeconds,
            }.ScheduleParallel();
        }

        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [BurstCompile]
        private partial struct SetCoolDownJob : IJobEntity
        {
            [ReadOnly] public float MaxCoolDownSeconds;

            [BurstCompile]
            void Execute(
                EnabledRefRO<NewlySpawnedTag> newlySpawnedTag
                , EnabledRefRW<IsInRePickUpCoolDown> isInRePickUpCoolDownTag
                , in PreviousPickerEntity previousPickerEntity
                , ref PreviousPickerPickupCoolDownSeconds coolDownSeconds)
            {
                if (!newlySpawnedTag.ValueRO) return;
                if (isInRePickUpCoolDownTag.ValueRO) return;
                if (previousPickerEntity.Value == Entity.Null) return;

                coolDownSeconds = this.MaxCoolDownSeconds;
                isInRePickUpCoolDownTag.ValueRW = true;
            }
        }

    }

}