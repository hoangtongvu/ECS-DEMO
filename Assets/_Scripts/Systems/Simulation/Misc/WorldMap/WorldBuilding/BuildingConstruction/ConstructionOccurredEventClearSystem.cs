using Components.Misc.WorldMap.WorldBuilding.BuildingConstruction;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Simulation.Misc.WorldMap.WorldBuilding.BuildingConstruction
{
    [UpdateInGroup(typeof(ConstructionOccurredEventHandleSystemGroup), OrderLast = true)]
    [BurstCompile]
    public partial struct ConstructionOccurredEventClearSystem : ISystem
    {
        private EntityQuery query0;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    ConstructionOccurredEvent>()
                .Build();

            state.RequireForUpdate(this.query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            em.SetComponentEnabled<ConstructionOccurredEvent>(this.query0, false);
        }

    }

}