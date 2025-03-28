using Components.Misc.WorldMap;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Initialization.Misc.WorldMap
{
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    public partial struct EntityGridSquareSizeBakeSystem : ISystem
    {
        private EntityQuery entityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    EntityGridSquareSize.ToRegisterEntity
                    , EntityGridSquareSize>()
                .WithOptions(EntityQueryOptions.IncludePrefab)
                .Build();

            state.RequireForUpdate(this.entityQuery);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var toRegisterEntities = this.entityQuery.ToComponentDataArray<EntityGridSquareSize.ToRegisterEntity>(Allocator.Temp);
            var gridSquareSizes = this.entityQuery.ToComponentDataArray<EntityGridSquareSize>(Allocator.Temp);
            var tempEntities = this.entityQuery.ToEntityArray(Allocator.Temp);

            int count = toRegisterEntities.Length;
            var em = state.EntityManager;

            for (int i = 0; i < count; i++)
            {
                em.AddComponentData(toRegisterEntities[i].Value, gridSquareSizes[i]);
            }

            em.DestroyEntity(tempEntities);

            toRegisterEntities.Dispose();
            gridSquareSizes.Dispose();
            tempEntities.Dispose();

        }

    }

}