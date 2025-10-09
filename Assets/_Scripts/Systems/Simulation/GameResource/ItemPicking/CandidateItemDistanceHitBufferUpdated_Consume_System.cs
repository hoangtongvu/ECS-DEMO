using Unity.Entities;
using Unity.Burst;
using Components.GameResource.ItemPicking.Picker;

namespace Systems.Simulation.GameResource.ItemPicking
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
    [BurstCompile]
    public partial struct CandidateItemDistanceHitBufferUpdated_Consume_System : ISystem
    {
        private EntityQuery query0;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    CandidateItemDistanceHitBufferUpdated>()
                .Build();

            state.RequireForUpdate(this.query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.SetComponentEnabled<CandidateItemDistanceHitBufferUpdated>(this.query0, false);
        }

    }

}