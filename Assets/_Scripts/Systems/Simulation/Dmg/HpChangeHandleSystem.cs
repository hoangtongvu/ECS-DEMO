using Aspect;
using Components.Damage;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Simulation
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct HpChangeHandleSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            // state.RequireForUpdate<DmgReceiverAspect>(); //Got exception when requiring aspect -> require per component instead.
            state.RequireForUpdate<HpComponent>();
            state.RequireForUpdate<HpChangeState>();
            state.RequireForUpdate<AliveState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            HpChangeJob hpChangeJob = new();
            hpChangeJob.ScheduleParallel();
        }


        [BurstCompile]
        private partial struct HpChangeJob : IJobEntity
        {
            void Execute(DmgReceiverAspect dmgReceiverAspect)
            {
                if (!dmgReceiverAspect.HpChangeStateRef.ValueRO.IsChanged) return;

                dmgReceiverAspect.HpChangeStateRef.ValueRW.IsChanged = false;

                int rawCurrentHp = dmgReceiverAspect.HpComponentRef.ValueRO.CurrentHp;
                rawCurrentHp += dmgReceiverAspect.HpChangeStateRef.ValueRO.ChangedValue;

                if (rawCurrentHp > dmgReceiverAspect.HpComponentRef.ValueRO.MaxHp)
                    dmgReceiverAspect.HpComponentRef.ValueRW.CurrentHp = dmgReceiverAspect.HpComponentRef.ValueRO.MaxHp;
                else if (rawCurrentHp <= 0)
                {
                    dmgReceiverAspect.HpComponentRef.ValueRW.CurrentHp = 0;
                    dmgReceiverAspect.AliveStateRef.ValueRW.Value = false;
                }
                else
                    dmgReceiverAspect.HpComponentRef.ValueRW.CurrentHp = rawCurrentHp;

            }
        }

        public static void DeductHp(ref HpChangeState hpChangeState, int deductValue)
        {
            hpChangeState.IsChanged = true;
            hpChangeState.ChangedValue = - deductValue;
        }

        public static void RecoverHp(ref HpChangeState hpChangeState, int healValue)
        {
            hpChangeState.IsChanged = true;
            hpChangeState.ChangedValue = healValue;
        }

    }
}