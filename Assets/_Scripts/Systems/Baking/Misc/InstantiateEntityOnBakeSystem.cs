using Components.Misc;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Baking.Misc
{
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    [BurstCompile]
    public partial struct InstantiateEntityOnBakeSystem : ISystem
    {
        private EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<InstantiateEntityOnBakeTag>()
                .WithAll<Prefab>()
                .Build();

            state.RequireForUpdate(query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.query.ToEntityArray(Allocator.Temp);

            int entityCount = entities.Length;

            for (int i = 0; i < entityCount; i++)
            {
                state.EntityManager.RemoveComponent<Prefab>(entities[i]);
            }

        }

    }

}