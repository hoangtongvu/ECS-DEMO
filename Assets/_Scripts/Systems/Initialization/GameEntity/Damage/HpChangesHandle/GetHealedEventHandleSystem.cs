using Components.GameEntity.Damage;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.GameEntity.Damage.HpChangesHandle
{
    [UpdateInGroup(typeof(HpChangesHandleSystemGroup))]
    [BurstCompile]
    public partial struct GetHealedEventHandleSystem : ISystem
    {
        private EntityQuery getHealedEventQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    GetHealedEvent
                    , FrameHpChange>()
                .WithAll<
                    IsAlive>()
                .Build();

            this.getHealedEventQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    GetHealedEvent>()
                .Build();

            state.RequireForUpdate(query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.SetComponentEnabled<GetHealedEvent>(this.getHealedEventQuery, false);

            foreach (var (frameHpChangeRef, entity) in SystemAPI
                .Query<
                    RefRO<FrameHpChange>>()
                .WithAll<
                    IsAlive>()
                .WithDisabled<
                    GetHealedEvent>()
                .WithEntityAccess())
            {
                if (frameHpChangeRef.ValueRO <= 0) continue;
                SystemAPI.SetComponentEnabled<GetHealedEvent>(entity, true);
            }

        }

    }

}