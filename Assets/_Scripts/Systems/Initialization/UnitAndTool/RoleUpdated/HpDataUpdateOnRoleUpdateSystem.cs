using Components.GameEntity;
using Components.GameEntity.Damage;
using Components.Unit;
using Components.Unit.Misc;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Initialization.UnitAndTool.RoleUpdated
{
    [UpdateInGroup(typeof(RoleUpdatedSystemGroup))]
    [UpdateAfter(typeof(PrimaryEntityUpdateOnRoleUpdateSystem))]
    [BurstCompile]
    public partial struct HpDataUpdateOnRoleUpdateSystem : ISystem
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
            state.RequireForUpdate<HpDataMap>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var hpDataMap = SystemAPI.GetSingleton<HpDataMap>().Value;

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (primaryPrefabEntityHolderRef, entity) in
                SystemAPI.Query<
                    RefRO<PrimaryPrefabEntityHolder>>()
                    .WithAll<NeedRoleUpdatedTag>()
                    .WithEntityAccess())
            {
                var hpData = hpDataMap[primaryPrefabEntityHolderRef.ValueRO];

                ecb.SetComponent(entity, new CurrentHp
                {
                    Value = hpData.MaxHp,
                });

                ecb.SetSharedComponent(entity, new HpDataHolder
                {
                    Value = hpData,
                });

            }

            ecb.Playback(state.EntityManager);

        }

    }

}