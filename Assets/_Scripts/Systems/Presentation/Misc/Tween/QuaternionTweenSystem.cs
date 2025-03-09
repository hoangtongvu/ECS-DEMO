using Unity.Entities;
using Unity.Burst;
using Components.Misc.Tween;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;

namespace Systems.Presentation.Misc.Tween
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [BurstCompile]
    public partial struct QuaternionTweenSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CanQuaternionTweenTag>();
            state.RequireForUpdate<QuaternionTweenData>();
        }

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
                EnabledRefRW<CanQuaternionTweenTag> canQuaternionTweenTag
                , ref LocalTransform transform
                , ref QuaternionTweenData quaternionTween)
            {
                const float epsilon = 0.01f;
                const float tweenDuration = 2f; // TODO: Find another way to assign duration.

                //if (quaternionTween.LifeTimeCounterSecond > tweenDuration)
                //{
                //    canQuaternionTweenTag.ValueRW = false;
                //    return;
                //}

                if (math.all(math.abs(quaternionTween.Target - transform.Rotation.value) < new float4(epsilon)))
                {
                    canQuaternionTweenTag.ValueRW = false;
                    return;
                }

                transform.Rotation =
                    math.lerp(transform.Rotation.value, quaternionTween.Target, quaternionTween.BaseSpeed * this.DeltaTime);

                quaternionTween.LifeTimeCounterSecond += this.DeltaTime;

            }

        }

    }

}