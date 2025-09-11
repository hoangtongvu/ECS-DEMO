using Components.GameEntity.Damage;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using TweenLib.StandardTweeners.ShakePositionTweeners;

namespace Systems.Initialization.Harvest
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct ShakeOnTakingHitSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    ShakePositionXZTweener_TweenData
                    , Can_ShakePositionXZTweener_TweenTag
                    , TakeHitEvent>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (tweenDataRef, canTweenTag, takeHitEventRef) in SystemAPI
                .Query<
                    RefRW<ShakePositionXZTweener_TweenData>
                    , EnabledRefRW<Can_ShakePositionXZTweener_TweenTag>
                    , EnabledRefRO<TakeHitEvent>>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                if (!takeHitEventRef.ValueRO) continue;

                ShakePositionXZTweener.TweenBuilder.Create(0.4f, new(15f, 0.4f, 0f))
                    .Build(ref tweenDataRef.ValueRW, canTweenTag);
            }
        }

    }

}