using Components.GameResource.ItemPicking;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Systems.Simulation.GameResource.ItemPicking
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct PickerPosUpdateSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    PickerEntity
                    , PickerPos>()
                .WithAll<
                    NeedUpdatePickerPos>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new UpdatePosJob
            {
                TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(),
            }.ScheduleParallel();
        }

        [BurstCompile]
        private partial struct UpdatePosJob : IJobEntity
        {
            [ReadOnly] public ComponentLookup<LocalTransform> TransformLookup;

            void Execute(
                EnabledRefRW<NeedUpdatePickerPos> needUpdatePickerPosTag
                , in PickerEntity pickerEntity
                , ref PickerPos pickerPos)
            {
                needUpdatePickerPosTag.ValueRW = false;
                this.TransformLookup.TryGetComponent(pickerEntity.Value, out var pickerTransform);
                pickerPos.Value = pickerTransform.Position;
            }
        }

    }

}