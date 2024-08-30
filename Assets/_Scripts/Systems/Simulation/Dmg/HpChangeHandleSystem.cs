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
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    HpComponent
                    , HpChangedTag
                    , HpChangedValue
                    , IsAliveTag>()
                .Build();

            state.RequireForUpdate(query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new HpChangeJob()
                .ScheduleParallel();
        }


        [BurstCompile]
        private partial struct HpChangeJob : IJobEntity
        {
            void Execute(
                ref HpComponent hpComponent
                , EnabledRefRW<HpChangedTag> hpChangedTag
                , ref HpChangedValue hpChangedValue
                , EnabledRefRW<IsAliveTag> aliveTag)
            {
                hpChangedTag.ValueRW = false;

                int rawCurrentHp = hpComponent.CurrentHp;
                rawCurrentHp += hpChangedValue.Value;

                if (rawCurrentHp > hpComponent.MaxHp)
                    hpComponent.CurrentHp = hpComponent.MaxHp;
                else if (rawCurrentHp <= 0)
                {
                    hpComponent.CurrentHp = 0;
                    aliveTag.ValueRW = false;
                }
                else
                    hpComponent.CurrentHp = rawCurrentHp;

            }
        }

        //Cant use EnabledRefRW with Burst inside a BurstCompiled function.
        //[BurstCompile]
        //public static void DeductHp(
        //    EnabledRefRW<HpChangedTag> hpChangedTag
        //    , ref HpChangedValue hpChangedValue
        //    , int deductValue)
        //{
        //    hpChangedTag.ValueRW = true;
        //    hpChangedValue.Value = -deductValue;
        //}

        //[BurstCompile]
        //public static void RecoverHp(
        //    EnabledRefRW<HpChangedTag> hpChangedTag
        //    , ref HpChangedValue hpChangedValue
        //    , int healValue)
        //{
        //    hpChangedTag.ValueRW = true;
        //    hpChangedValue.Value = healValue;
        //}

    }
}