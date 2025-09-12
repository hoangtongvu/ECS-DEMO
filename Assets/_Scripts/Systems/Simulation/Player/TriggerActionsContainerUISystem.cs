using Components.GameEntity.InteractableActions;
using Components.GameEntity.Reaction;
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
                .WithDisabled<
                    RunReaction.UpdatingTag>()
                .Build();

            state.RequireForUpdate(this.playerQuery);
            state.RequireForUpdate<NearestInteractableEntity>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.CompleteDependencyBeforeRW<PhysicsWorldSingleton>();

            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var nearestInteractableEntityRef = SystemAPI.GetSingletonRW<NearestInteractableEntity>();

            var currentNearestEntity = nearestInteractableEntityRef.ValueRO.Value;
            bool canFindPlayer = this.playerQuery.CalculateEntityCount() != 0;

            if (canFindPlayer)
            {
                var playerTransform = this.playerQuery.GetSingleton<LocalTransform>();
                half playerInteractRadius = this.playerQuery.GetSingleton<PlayerInteractRadius>().Value;

                var hitList = new NativeList<DistanceHit>(5, Allocator.Temp);
                this.OverlapSphere(in physicsWorld, in playerTransform.Position, in playerInteractRadius, ref hitList);

                hitList.Sort(new DistanceHitComparer
                {
                    BasePos = playerTransform.Position,
                });

                foreach (var hit in hitList)
                {
                    var newNearestEntity = hit.Entity;
                    if (!SystemAPI.HasComponent<EntitySupportsShowActionsContainerUI>(newNearestEntity)) continue;

                    if (currentNearestEntity != Entity.Null)
                    {
                        if (currentNearestEntity == newNearestEntity) return;

                        bool isValidEntity = SystemAPI.HasComponent<IsTargetForActionsContainerUI>(currentNearestEntity);
                        if (isValidEntity)
                        {
                            SystemAPI.SetComponentEnabled<IsTargetForActionsContainerUI>(currentNearestEntity, false);
                        }
                    }

                    SystemAPI.SetComponentEnabled<IsTargetForActionsContainerUI>(newNearestEntity, true);
                    nearestInteractableEntityRef.ValueRW.Value = newNearestEntity;
                    this.SetEnabledActionsContainerUI(ref state, true);

                    return;
                }
            }

            this.HandleOldInvalidTargetEntity(ref state, ref nearestInteractableEntityRef.ValueRW);
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

        [BurstCompile]
        private void SetEnabledActionsContainerUI(ref SystemState state, bool enabledState)
        {
            foreach (var (canShowTag, canUpdateTag) in SystemAPI
                    .Query<
                        EnabledRefRW<ActionsContainerUI_CD.CanShow>
                        , EnabledRefRW<ActionsContainerUI_CD.CanUpdate>>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                canShowTag.ValueRW = enabledState;
                canUpdateTag.ValueRW = enabledState;
            }
        }

        [BurstCompile]
        private void HandleOldInvalidTargetEntity(
            ref SystemState state
            , ref NearestInteractableEntity nearestInteractableEntity)
        {
            var currentNearestEntity = nearestInteractableEntity.Value;
            if (currentNearestEntity == Entity.Null) return;

            bool isValidEntity = SystemAPI.HasComponent<IsTargetForActionsContainerUI>(currentNearestEntity);
            if (isValidEntity)
            {
                SystemAPI.SetComponentEnabled<IsTargetForActionsContainerUI>(currentNearestEntity, false);
            }

            nearestInteractableEntity.Value = Entity.Null;
            this.SetEnabledActionsContainerUI(ref state, false);
        }

    }

}