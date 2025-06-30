using Components.Misc.WorldMap.WorldBuilding;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Simulation.Misc.WorldMap.WorldBuilding.BuildingConstruction
{
    [UpdateInGroup(typeof(EndConstructionProcessSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(EndConstructionProcessSystem))]
    [BurstCompile]
    public partial struct ConstructionNewlyEndedTagClearSystem : ISystem
    {
        private EntityQuery entityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    ConstructionNewlyEndedTag>()
                .Build();

            state.RequireForUpdate(this.entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.entityQuery.ToEntityArray(Allocator.Temp);
            state.EntityManager.RemoveComponent<ConstructionNewlyEndedTag>(entities);
        }

    }

}