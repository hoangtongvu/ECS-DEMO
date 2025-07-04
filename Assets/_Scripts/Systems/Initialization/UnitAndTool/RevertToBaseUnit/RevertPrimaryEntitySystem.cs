using Components.GameEntity;
using Components.Unit;
using Components.Unit.Misc;
using Components.Unit.RevertToBaseUnit;
using Systems.Initialization.UnitAndTool.RevertToBaseUnit.RevertRole;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Initialization.UnitAndTool.RevertToBaseUnit
{
    [UpdateInGroup(typeof(RevertToBaseUnitSystemGroup))]
    [UpdateAfter(typeof(RevertRoleSystemGroup))]
    [BurstCompile]
    public partial struct RevertPrimaryEntitySystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitProfileIdHolder
                    , PrimaryPrefabEntityHolder
                    , NeedRevertToBaseUnitTag>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate<UnitProfileId2PrimaryPrefabEntityMap>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var unitProfileId2PrimaryPrefabEntityMap = SystemAPI.GetSingleton<UnitProfileId2PrimaryPrefabEntityMap>();

            state.Dependency = new UpdatePrimaryPrefabEntitiesJob
            {
                UnitProfileId2PrimaryPrefabEntityMap = unitProfileId2PrimaryPrefabEntityMap,
            }.ScheduleParallel(state.Dependency);

        }

        [WithAll(typeof(NeedRevertToBaseUnitTag))]
        [BurstCompile]
        private partial struct UpdatePrimaryPrefabEntitiesJob : IJobEntity
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