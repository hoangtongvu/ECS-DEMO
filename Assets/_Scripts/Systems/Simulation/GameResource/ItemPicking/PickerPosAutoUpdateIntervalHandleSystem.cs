using Components.GameResource.ItemPicking;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Systems.Simulation.GameResource.ItemPicking
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct PickerPosAutoUpdateIntervalHandleSystem : ISystem
    {
        private EntityQuery query0;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query0 = SystemAPI.QueryBuilder()
                .WithAll<IsCandidateItem>()
                .WithDisabled<NeedUpdatePickerPos>()
                .Build();

            state.RequireForUpdate(this.query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.query0.ToEntityArray(Allocator.TempJob);
            int length = entities.Length;

            if (length == 0)
            {
                entities.Dispose();
                return;
            }

            state.Dependency = new AutoUpdateHandleJob
            {
                ItemEntities = entities,
                NeedUpdatePickerPosLookup = SystemAPI.GetComponentLookup<NeedUpdatePickerPos>(),
            }.ScheduleParallel(length, 32, state.Dependency);
        }

        [BurstCompile]
        private struct AutoUpdateHandleJob : IJobParallelForBatch
        {
            [ReadOnly]
            [DeallocateOnJobCompletion]
            public NativeArray<Entity> ItemEntities;

            [NativeDisableParallelForRestriction]
            public ComponentLookup<NeedUpdatePickerPos> NeedUpdatePickerPosLookup;

            public void Execute(int startIndex, int count)
            {
                int upperBound = startIndex + count;

                for (int i = startIndex; i < upperBound; i++)
                {
                    var itemEntity = this.ItemEntities[i];
                    this.NeedUpdatePickerPosLookup.SetComponentEnabled(itemEntity, true);
                }
            }
        }

    }

}