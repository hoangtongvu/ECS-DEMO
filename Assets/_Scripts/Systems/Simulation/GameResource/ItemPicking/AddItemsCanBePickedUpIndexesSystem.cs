using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Components.GameResource.ItemPicking.Picker;

namespace Systems.Simulation.GameResource.ItemPicking
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(GatherCandidateItemsSystem))]
    [BurstCompile]
    public partial struct AddItemsCanBePickedUpIndexesSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    CandidateItemDistanceHit
                    , CandidateItemDistanceHitBufferUpdated
                    , ItemCanBePickedUpIndex>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            const float interactRadius = 0.5f;

            new IndexesAddJob
            {
                PickUpRadius = interactRadius,
            }.ScheduleParallel();
        }

        [WithAll(typeof(CandidateItemDistanceHitBufferUpdated))]
        [BurstCompile]
        private partial struct IndexesAddJob : IJobEntity
        {
            [ReadOnly] public float PickUpRadius;

            void Execute(
                in DynamicBuffer<CandidateItemDistanceHit> candidateItemDistanceHits
                , ref DynamicBuffer<ItemCanBePickedUpIndex> itemCanBePickedUpIndices)
            {
                int length = candidateItemDistanceHits.Length;
                if (length == 0) return;

                for (int i = 0; i < length; i++)
                {
                    var distanceHit = candidateItemDistanceHits[i];
                    if (distanceHit.Distance > this.PickUpRadius) continue;

                    itemCanBePickedUpIndices.Add(i);
                }
            }
        }

    }

}