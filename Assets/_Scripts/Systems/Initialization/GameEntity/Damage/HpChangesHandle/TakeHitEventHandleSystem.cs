using Components.GameEntity.Damage;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.GameEntity.Damage.HpChangesHandle
{
    [UpdateInGroup(typeof(HpChangesHandleSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(HpChangesHandleSystem))]
    [BurstCompile]
    public partial struct TakeHitEventHandleSystem : ISystem
    {
        private EntityQuery takeHitEventQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    TakeHitEvent
                    , FrameHpChange>()
                .WithAll<
                    IsAlive>()
                .Build();

            this.takeHitEventQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    TakeHitEvent>()
                .Build();

            state.RequireForUpdate(query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.SetComponentEnabled<TakeHitEvent>(this.takeHitEventQuery, false);

            foreach (var (frameHpChangeRef, entity) in SystemAPI
                .Query<
                    RefRO<FrameHpChange>>()
                .WithAll<
                    IsAlive>()
                .WithEntityAccess())
            {
                if (frameHpChangeRef.ValueRO.Value >= 0) continue;
                SystemAPI.SetComponentEnabled<TakeHitEvent>(entity, true);
            }

        }

    }

}