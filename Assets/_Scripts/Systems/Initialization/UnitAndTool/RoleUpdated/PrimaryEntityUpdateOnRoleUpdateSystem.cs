using Components.GameEntity;
using Components.Unit;
using Components.Unit.Misc;
using Systems.Initialization.UnitAndTool.RoleUpdated.InitRoleComponents;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Initialization.UnitAndTool.RoleUpdated
{
    [UpdateInGroup(typeof(RoleUpdatedSystemGroup))]
    [UpdateAfter(typeof(InitRoleComponentsSystemGroup))]
    [BurstCompile]
    public partial struct PrimaryEntityUpdateOnRoleUpdateSystem : ISystem
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
            state.RequireForUpdate<UnitProfileId2PrimaryPrefabEntityMap>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var unitProfileId2PrimaryPrefabEntityMap = SystemAPI.GetSingleton<UnitProfileId2PrimaryPrefabEntityMap>();

            state.Dependency = new PrimaryPrefabEntitiesUpdatedJob
            {
                UnitProfileId2PrimaryPrefabEntityMap = unitProfileId2PrimaryPrefabEntityMap,
            }.ScheduleParallel(state.Dependency);

        }

        [WithAll(typeof(NeedRoleUpdatedTag))]
        [BurstCompile]
        private partial struct PrimaryPrefabEntitiesUpdatedJob : IJobEntity
        {
            [ReadOnly] public UnitProfileId2PrimaryPrefabEntityMap UnitProfileId2PrimaryPrefabEntityMap;

            [BurstCompile]
            void Execute(
                in UnitProfileIdHolder unitProfileIdHolder
                , ref PrimaryPrefabEntityHolder primaryPrefabEntityHolder)
            {
                primaryPrefabEntityHolder = new(this.UnitProfileId2PrimaryPrefabEntityMap.Value[unitProfileIdHolder.Value]);
            }

        }

    }

}