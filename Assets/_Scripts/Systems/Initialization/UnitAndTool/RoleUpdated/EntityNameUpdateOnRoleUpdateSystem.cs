using Components.GameEntity;
using Components.Unit;
using Components.Unit.Misc;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.UnitAndTool.RoleUpdated
{
    [UpdateInGroup(typeof(RoleUpdatedSystemGroup))]
    [UpdateAfter(typeof(PrimaryEntityUpdateOnRoleUpdateSystem))]
    [BurstCompile]
    public partial struct EntityNameUpdateOnRoleUpdateSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitProfileIdHolder
                    , PrimaryPrefabEntityHolder
                    , NeedRoleUpdatedTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;

            foreach (var (primaryPrefabEntityHolderRef, entity) in
                SystemAPI.Query<
                    RefRO<PrimaryPrefabEntityHolder>>()
                    .WithAll<NeedRoleUpdatedTag>()
                    .WithEntityAccess())
            {
                em.GetName(primaryPrefabEntityHolderRef.ValueRO, out var name64Bytes);
                em.SetName(entity, name64Bytes);
            }

        }

    }

}