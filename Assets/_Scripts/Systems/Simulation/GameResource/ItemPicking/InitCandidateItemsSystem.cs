using Components.GameEntity.Interaction.InteractionPhases;
using Components.GameResource.ItemPicking.Pickee;
using Components.GameResource.ItemPicking.Picker;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Simulation.GameResource.ItemPicking
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct InitCandidateItemsSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    CandidateItemDistanceHit>()
                .WithAll<
                    PreInteractionPhase.StartedEvent>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (candidateItemDistanceHits, pickerEntity) in SystemAPI
                .Query<
                    DynamicBuffer<CandidateItemDistanceHit>>()
                .WithAll<
                    CandidateItemDistanceHitBufferUpdated>()
                .WithEntityAccess())
            {
                foreach (var distanceHit in candidateItemDistanceHits)
                {
                    var itemEntity = distanceHit.Entity;

                    if (SystemAPI.IsComponentEnabled<IsCandidateItem>(itemEntity)) continue;

                    SystemAPI.SetComponentEnabled<IsCandidateItem>(itemEntity, true);
                    SystemAPI.SetComponent(itemEntity, new PickerEntity
                    {
                        Value = pickerEntity,
                    });
                    SystemAPI.SetComponentEnabled<NeedUpdatePickerPos>(itemEntity, true);
                }
            }
        }

    }

}