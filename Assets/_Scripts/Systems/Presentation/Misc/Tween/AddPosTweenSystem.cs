using Unity.Entities;
using Unity.Burst;
using Components.Misc.Tween;
using Unity.Collections;
using Unity.Mathematics;
using Components.Camera;

namespace Systems.Presentation.Misc.Tween
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [BurstCompile]
    public partial struct AddPosTweenSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CanAddPosTweenTag>();
            state.RequireForUpdate<AddPosTweenData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            new TweenJob
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
            }.ScheduleParallel();

        }

        [BurstCompile]
        private partial struct TweenJob : IJobEntity
        {
            [ReadOnly] public float DeltaTime;

            [BurstCompile]
            void Execute(
                EnabledRefRW<CanAddPosTweenTag> canTweenTag
                , ref AddPos addPos
                , ref AddPosTweenData tweenData)
            {
                const float epsilon = 0.01f;

                float3 target = tweenData.Value.Target.Float3;

                if (math.all(math.abs(target - addPos.Value) < new float3(epsilon)))
                {
                    canTweenTag.ValueRW = false;
                    return;
                }
                
                addPos.Value =
                    math.lerp(addPos.Value, target, tweenData.Value.BaseSpeed * this.DeltaTime);

                tweenData.Value.LifeTimeCounterSecond += this.DeltaTime;

            }

        }

    }

}