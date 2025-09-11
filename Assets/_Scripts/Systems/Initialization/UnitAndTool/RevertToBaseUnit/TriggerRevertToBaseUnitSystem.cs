using Components.GameEntity.Damage;
using Components.Unit;
using Components.Unit.Misc;
using Components.Unit.RevertToBaseUnit;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.UnitAndTool.RevertToBaseUnit
{
    [UpdateInGroup(typeof(RevertToBaseUnitSystemGroup), OrderFirst = true)]
    [BurstCompile]
    public partial struct TriggerRevertToBaseUnitSystem : ISystem
    {
        private EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitTag>()
                .WithNone<
                    JoblessUnitTag>()
                .WithAll<
                    PendingDead>()
                .Build();

            state.RequireForUpdate(this.query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.AddComponent<NeedRevertToBaseUnitTag>(this.query);
        }

    }

}
