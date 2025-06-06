using Components.Tool.Misc;
using Components.Unit;
using Components.Unit.Misc;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Initialization.Tool.InitArmedStateComponents
{
    [UpdateInGroup(typeof(InitArmedStateComponentsSystemGroup), OrderLast = true)]
    [BurstCompile]
    public partial struct ClearNeedInitArmedStateComponentsTagSystem : ISystem
    {
        private EntityQuery entityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    ToolProfileIdHolder
                    , UnitProfileIdHolder
                    , NeedInitArmedStateComponentsTag>()
                .Build();

            state.RequireForUpdate(this.entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.entityQuery.ToEntityArray(Allocator.Temp);
            state.EntityManager.RemoveComponent<NeedInitArmedStateComponentsTag>(entities);

            entities.Dispose();

        }

    }

}