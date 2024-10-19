using Unity.Entities;
using Unity.Burst;
using Components;
using Components.Damage;
using Components.Unit;
using Components.Misc.GlobalConfigs;

namespace Systems.Simulation.Unit
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct UnitPatrolSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var gameGlobalConfigs = SystemAPI.GetSingleton<GameGlobalConfigsICD>();

            foreach (var (idleTimeCounterRef, canMoveEntityTag, entity) in
                SystemAPI.Query<
                    RefRW<UnitIdleTimeCounter>
                    , EnabledRefRW<CanMoveEntityTag>>()
                    .WithAll<IsAliveTag>()
                    .WithDisabled<IsUnitWorkingTag>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)
                    .WithEntityAccess())
            {
                // Count the timer for idle state
                // On Idle end -> Make Unit to walk to a random position around
                // No MoveCommand left -> auto change CanMoveEntityTag disabled -> Idle state start again


                if (!canMoveEntityTag.ValueRO)
                {
                    // Also counter = 0??
                    idleTimeCounterRef.ValueRW.Value += SystemAPI.Time.DeltaTime;
                }


                if (idleTimeCounterRef.ValueRO.Value >= gameGlobalConfigs.Value.UnitIdleMaxDuration)
                {
                    idleTimeCounterRef.ValueRW.Value = 0;
                    SystemAPI.SetComponentEnabled<NeedsInitWalkTag>(entity, true);
                }

            }


        }




    }
}