using Components.Misc.WorldMap.WorldBuilding;
using Components.Misc.WorldMap.WorldBuilding.BuildingConstruction;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Simulation.Misc.WorldMap.WorldBuilding.BuildingConstruction
{
    [UpdateInGroup(typeof(EndConstructionProcessSystemGroup), OrderFirst = true)]
    [BurstCompile]
    public partial struct EndConstructionProcessSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    ConstructionRemaining>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = new NativeList<Entity>(10, Allocator.Temp);

            foreach (var (constructionRemainingRef, entity) in SystemAPI
                .Query<
                    RefRO<ConstructionRemaining>>()
                .WithEntityAccess())
            {
                bool constructionEnded = constructionRemainingRef.ValueRO.Value == uint.MinValue;
                if (!constructionEnded) continue;

                entities.Add(entity);
            }

            var em = state.EntityManager;
            var entityArray = entities.AsArray();

            em.RemoveComponent<ConstructionRemaining>(entityArray);
            em.RemoveComponent<ConstructionOccurredEvent>(entityArray);
            em.AddComponent<ConstructionNewlyEndedTag>(entityArray);
        }

    }

}