using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Transforms;
using Unity.Physics;
using Core.Misc;
using Components.GameResource.ItemPicking;

namespace Systems.Simulation.GameResource.ItemPicking
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct GatherCandidateItemsSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    LocalTransform
                    , CandidateItemDistanceHit
                    , CandidateItemDistanceHitBufferUpdated>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            const float interactRadius = 3f;

            foreach (var (transformRef, candidateItemDistanceHits, pickerEntity) in SystemAPI
                .Query<
                    RefRO<LocalTransform>
                    , DynamicBuffer<CandidateItemDistanceHit>>()
                .WithEntityAccess())
            {
                var hitList = new NativeList<DistanceHit>(Allocator.Temp);

                bool hasHit = physicsWorld.OverlapSphere(
                    transformRef.ValueRO.Position
                    , interactRadius
                    , ref hitList
                    , this.GetCollisionFilter());

                if (!hasHit) continue;

                candidateItemDistanceHits.Clear();

                foreach (var hit in hitList)
                {
                    bool isPickableItem = SystemAPI.HasComponent<PickableItem>(hit.Entity) && SystemAPI.IsComponentEnabled<PickableItem>(hit.Entity);

                    if (!isPickableItem) continue;

                    candidateItemDistanceHits.Add(hit);
                }

                SystemAPI.SetComponentEnabled<CandidateItemDistanceHitBufferUpdated>(pickerEntity, true);
            }

        }

        [BurstCompile]
        private CollisionFilter GetCollisionFilter()
        {
            return new CollisionFilter
            {
                BelongsTo = (uint)(CollisionLayer.Unit | CollisionLayer.Player),
                CollidesWith = (uint)(CollisionLayer.Item),
            };
        }

    }

}