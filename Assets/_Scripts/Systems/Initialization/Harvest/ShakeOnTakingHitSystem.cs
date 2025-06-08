using Components.GameEntity.Damage;
using TweenLib.ShakeTween.Data;
using TweenLib.ShakeTween.Logic;
using TweenLib.Timer.Data;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

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
                    , ShakeDataIdHolder>()
                .WithAll<IsAliveTag>()
                .Build();

            state.RequireForUpdate(query0);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.CompleteDependencyBeforeRW<TimerList>();
            state.EntityManager.CompleteDependencyBeforeRW<TimerIdPool>();
            state.EntityManager.CompleteDependencyBeforeRW<ShakeDataList>();
            state.EntityManager.CompleteDependencyBeforeRW<ShakeDataIdPool>();

            var timerList = SystemAPI.GetSingleton<TimerList>();
            var timerIdPool = SystemAPI.GetSingleton<TimerIdPool>();
            var shakeDataList = SystemAPI.GetSingleton<ShakeDataList>();
            var shakeDataIdPool = SystemAPI.GetSingleton<ShakeDataIdPool>();

            foreach (var (hpChangeRecords, transformRef, shakeDataIdHolderRef) in
                SystemAPI.Query<
                    DynamicBuffer<HpChangeRecordElement>
                    , RefRO<LocalTransform>
                    , RefRW<ShakeDataIdHolder>>()
                    .WithAll<IsAliveTag>())
            {
                if (hpChangeRecords.IsEmpty) continue;

                ShakeBuilder.Create(0.5f, 15f, 0.2f, transformRef.ValueRO.Position)
                    .Build(ref timerList, in timerIdPool, ref shakeDataList, in shakeDataIdPool, ref shakeDataIdHolderRef.ValueRW);

            }

        }

    }

}