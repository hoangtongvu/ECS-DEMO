using Components.Unit;
using Components.Unit.Misc;
using Components.Unit.RevertToBaseUnit;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Initialization.UnitAndTool.RevertToBaseUnit
{
    [UpdateInGroup(typeof(RevertToBaseUnitSystemGroup))]
    [BurstCompile]
    public partial struct RevertJoblessUnitTagSystem : ISystem
    {
        private EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitTag
                    , NeedRevertToBaseUnitTag>()
                .Build();

            state.RequireForUpdate(this.query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.query.ToEntityArray(Allocator.Temp);
            state.EntityManager.AddComponent<JoblessUnitTag>(entities);
        }

    }

}
