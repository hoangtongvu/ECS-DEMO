using Components.GameEntity.Misc;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Initialization.GameEntity.Misc
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct DestroyEntityWithTagSystem : ISystem
    {
        private EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    NeedDestroyEntityTag>()
                .Build();

            state.RequireForUpdate(this.query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.query.ToEntityArray(Allocator.Temp);
            if (entities.Length == 0) return;

            state.EntityManager.DestroyEntity(entities);

        }

    }

}