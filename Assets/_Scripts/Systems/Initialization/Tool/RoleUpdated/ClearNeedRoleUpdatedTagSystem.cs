using Components.Tool.Misc;
using Components.Unit;
using Components.Unit.Misc;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Initialization.Tool.RoleUpdated
{
    [UpdateInGroup(typeof(RoleUpdatedSystemGroup), OrderLast = true)]
    [BurstCompile]
    public partial struct ClearNeedRoleUpdatedTagSystem : ISystem
    {
        private EntityQuery entityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    ToolProfileIdHolder
                    , UnitProfileIdHolder
                    , NeedRoleUpdatedTag>()
                .Build();

            state.RequireForUpdate(this.entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.entityQuery.ToEntityArray(Allocator.Temp);
            state.EntityManager.RemoveComponent<NeedRoleUpdatedTag>(entities);

        }

    }

}