using Unity.Entities;
using Components.Player;
using Unity.Burst;
using Components.Misc;

namespace Systems.Simulation.Player
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct AttackInputHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AttackData>();
            state.RequireForUpdate<AttackInput>();
            state.RequireForUpdate<InputData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            bool hardwareInputState = SystemAPI.GetSingleton<InputData>().LeftMouseData.Down;

            foreach (var (attackDataRef, attackInputRef) in
                SystemAPI.Query<
                    RefRO<AttackData>
                    , RefRW<AttackInput>>())
            {
                attackInputRef.ValueRW.IsAttackable = hardwareInputState && !attackDataRef.ValueRO.isAttacking;
            }

        }

    }

}