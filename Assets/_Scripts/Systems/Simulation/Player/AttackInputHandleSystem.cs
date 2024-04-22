using Unity.Entities;
using Components.Player;
using Components;
using Unity.Burst;


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

            var setAttackInputJob = new SetAttackInputJob
            {
                hardwareInputState = SystemAPI.GetSingleton<InputData>().LeftMouseData.Down,
            };

            setAttackInputJob.ScheduleParallel();

        }

        [BurstCompile]
        private partial struct SetAttackInputJob : IJobEntity
        {
            public bool hardwareInputState;

            void Execute(
                in AttackData attackData
                , ref AttackInput attackInput)
            {
                attackInput.IsAttackable = this.hardwareInputState && !attackData.isAttacking;
            }

        }

    }
}

