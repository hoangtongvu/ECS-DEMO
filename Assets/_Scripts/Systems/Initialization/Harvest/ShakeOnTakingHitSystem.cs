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
                    HpChangeRecordElement
                    , LocalTransform
                    , ShakePositionXZTweener_TweenData
                    , Can_ShakePositionXZTweener_TweenTag>()
                .WithAll<IsAliveTag>()
                .Build();

            state.RequireForUpdate(query0);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (hpChangeRecords, transformRef, tweenDataRef, canTweenTag, isAliveTag) in
                SystemAPI.Query<
                    DynamicBuffer<HpChangeRecordElement>
                    , RefRO<LocalTransform>
                    , RefRW<ShakePositionXZTweener_TweenData>
                    , EnabledRefRW<Can_ShakePositionXZTweener_TweenTag>
                    , EnabledRefRO<IsAliveTag>>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))

            {
                if (!isAliveTag.ValueRO) continue;
                if (hpChangeRecords.IsEmpty) continue;

                ShakePositionXZTweener.TweenBuilder.Create(0.4f, new(15f, 0.4f, 0f))
                    .Build(ref tweenDataRef.ValueRW, canTweenTag);

            }

        }

    }

}