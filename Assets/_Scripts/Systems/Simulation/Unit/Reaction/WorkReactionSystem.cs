using Unity.Entities;
using Unity.Burst;
using Components.Unit;
using Components;
using Utilities.Extensions;
using Components.MyEntity;
using Components.Damage;
using Components.Unit.Reaction;

namespace Systems.Simulation.Unit.Reaction
{

    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [BurstCompile]
    public partial struct WorkReactionSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    WorkStartedTag
                    , InteractingEntity
                    , IsUnitWorkingTag
                    , IsAliveTag
                    , AnimatorData>()
                .Build();

            state.RequireForUpdate(query0);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            foreach (var (workStartedTag, interactingEntityRef, isUnitWorkingTag, isAliveTag, animatorDataRef) in
                SystemAPI.Query<
                    EnabledRefRW<WorkStartedTag>
                    , RefRO <InteractingEntity>
                    , EnabledRefRW<IsUnitWorkingTag>
                    , EnabledRefRO<IsAliveTag>
                    , RefRW<AnimatorData>>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                bool workTargetValid = interactingEntityRef.ValueRO.Value != Entity.Null;
                bool canUpdate = isAliveTag.ValueRO && workTargetValid;
                bool reactionStarted = workStartedTag.ValueRO;

                if (canUpdate)
                {
                    if (!reactionStarted)
                    {
                        //UnityEngine.Debug.Log("Start");
                        isUnitWorkingTag.ValueRW = true;
                        animatorDataRef.ValueRW.Value.ChangeValue("1H_Melee_Attack_Chop");
                        workStartedTag.ValueRW = true;
                    }

                    //UnityEngine.Debug.Log("Update");

                }

                if (!canUpdate && reactionStarted)
                {
                    //UnityEngine.Debug.Log("End");
                    isUnitWorkingTag.ValueRW = false;
                    workStartedTag.ValueRW = false;
                }


            }


        }


    }
}