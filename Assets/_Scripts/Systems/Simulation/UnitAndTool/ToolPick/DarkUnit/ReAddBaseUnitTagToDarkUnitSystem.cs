using Components.Unit;
using Components.Unit.DarkUnit;
using Components.Unit.Misc;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Simulation.UnitAndTool.ToolPick.DarkUnit
{
    [UpdateInGroup(typeof(ToolPickHandleSystemGroup))]
    [UpdateAfter(typeof(HandleUnitOnToolPickSystem))]
    [BurstCompile]
    public partial struct ReAddBaseUnitTagToDarkUnitSystem : ISystem
    {
        private EntityQuery unitQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.unitQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitTag
                    , DarkUnitTag>()
                .WithNone<
                    JoblessUnitTag>()
                .Build();

            state.RequireForUpdate(this.unitQuery);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            var unitEntities = this.unitQuery.ToEntityArray(Allocator.Temp);

            em.AddComponent<JoblessUnitTag>(unitEntities);
        }

    }

}