using Unity.Entities;
using UnityEngine;
using Components.Player;


namespace Systems.Simulation.Player
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class AttackInputHandleSystem : SystemBase
    {

        protected override void OnCreate()
        {
            RequireForUpdate<AttackData>();
            RequireForUpdate<AttackInput>();
        }

        protected override void OnUpdate()
        {

            SetAttackInputJob setAttackInputJob = new SetAttackInputJob
            {
                hardwareInputState = Input.GetMouseButtonDown(0),
            };

            setAttackInputJob.ScheduleParallel();

        }


        private partial struct SetAttackInputJob : IJobEntity
        {
            public bool hardwareInputState;

            void Execute(in AttackData attackData, ref AttackInput attackInput)
            {
                attackInput.IsAttackable = this.hardwareInputState && !attackData.isAttacking;
            }

        }

    }
}

