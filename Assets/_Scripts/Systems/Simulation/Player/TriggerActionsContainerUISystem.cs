using Components.GameEntity.InteractableActions;
using Components.Player;
using Core.Misc;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Systems.Simulation.Player
{
    [BurstCompile]
    public struct DistanceHitComparer : IComparer<DistanceHit>
    {
        public float3 BasePos;

        [BurstCompile]
        public readonly int Compare(DistanceHit a, DistanceHit b)
        {
            float distA = math.lengthsq(a.Position - BasePos);
            float distB = math.lengthsq(b.Position - BasePos);
            return distA.CompareTo(distB); // ascending order
        }
    }

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct TriggerActionsContainerUISystem : ISystem
    {
        private EntityQuery playerQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.playerQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerTag
                    , LocalTransform
                    , PlayerInteractRadius>()
                .WithAllRW<NearestInteractableEntity>()
                .Build();

            state.RequireForUpdate(this.playerQuery);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.CompleteDependencyBeforeRW<PhysicsWorldSingleton>();

            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var playerTransform = this.playerQuery.GetSingleton<LocalTransform>();
            var nearestInteractableEntityRef = this.playerQuery.GetSingletonRW<NearestInteractableEntity>();
            half playerInteractRadius = this.playerQuery.GetSingleton<PlayerInteractRadius>().Value;

            var hitList = new NativeList<DistanceHit>(5, Allocator.Temp);
            this.OverlapSphere(in physicsWorld, in playerTransform.Position, in playerInteractRadius, ref hitList);

            var currentNearestEntity = nearestInteractableEntityRef.ValueRO.Value;

            if (hitList.IsEmpty)
            {
                if (currentNearestEntity == Entity.Null) return;

                SystemAPI.SetComponentEnabled<CanShowActionsContainerUITag>(currentNearestEntity, false);
                nearestInteractableEntityRef.ValueRW.Value = Entity.Null;
                return;

            }

            hitList.Sort(new DistanceHitComparer
            {
                BasePos = playerTransform.Position,
            });

            var newNearestEntity = hitList[0].Entity;

            if (currentNearestEntity != Entity.Null)
            {
                if (currentNearestEntity == newNearestEntity) return;
                SystemAPI.SetComponentEnabled<CanShowActionsContainerUITag>(currentNearestEntity, false);
            }

            SystemAPI.SetComponentEnabled<CanShowActionsContainerUITag>(newNearestEntity, true);
            nearestInteractableEntityRef.ValueRW.Value = newNearestEntity;

        }

        [BurstCompile]
        private void OverlapSphere(
            in PhysicsWorldSingleton physicsWorld
            , in float3 centerPos
            , in half radius
            , ref NativeList<DistanceHit> hitList)
        {
            var collisionFilter = new CollisionFilter
            {
                BelongsTo = (uint)CollisionLayer.Player,
                CollidesWith = (uint)(CollisionLayer.Unit | CollisionLayer.Building),
            };

            physicsWorld.OverlapSphere(centerPos, radius, ref hitList, collisionFilter);
        }

    }

}