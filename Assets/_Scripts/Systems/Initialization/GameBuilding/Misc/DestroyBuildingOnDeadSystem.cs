using Components.GameBuilding;
using Components.GameEntity.Damage;
using Components.GameEntity.Misc;
using Components.GameEntity.Misc.EntityCleanup;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Utilities.Extensions;

namespace Systems.Initialization.GameBuilding.Misc
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct DestroyBuildingOnDeadSystem : ISystem
    {
        private EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    GameBuildingTag>()
                .WithDisabled<
                    IsAliveTag>()
                .Build();

            state.RequireForUpdate(this.query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.query.ToEntityArray(Allocator.Temp);
            if (entities.Length == 0) return;

            var em = state.EntityManager;

            em.AddComponent<NeedDestroyBasePresenterTag>(entities);
            em.AddComponent<NeedDestroyEntityTag>(entities);
            em.AddComponent<PendingCleanupEntityTag>(entities);
            em.AddComponentData(entities, new PendingCleanupEntityTimer
            {
                TimeStamp = SystemAPI.Time.ElapsedTime,
                DurationSeconds = new(2.5f),
            });

        }

    }

}