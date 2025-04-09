using Unity.Entities;
using Components.Unit.UnitSelection;
using Unity.Mathematics;
using Components;
using Unity.Physics;
using Core.Utilities.Extensions;
using Core;
using Components.Unit;
using Unity.Transforms;
using Unity.Burst;

namespace Systems.Presentation.Unit
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [BurstCompile]
    public partial struct TargetPosMarkerSpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    CurrentWorldWaypoint
                    , TargetPosMarkerHolder
                    , NewlySelectedUnitTag
                    , NewlyDeselectedUnitTag>()
                .Build();

            state.RequireForUpdate(query);
            state.RequireForUpdate<TargetPosMarkerPrefab>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var targetPosMarkerPrefab = SystemAPI.GetSingleton<TargetPosMarkerPrefab>();
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);


            foreach (var (currentWaypointRef, targetPosMarkerHolderRef) in
                SystemAPI.Query<
                    RefRO<CurrentWorldWaypoint>
                    , RefRW<TargetPosMarkerHolder>>()
                    .WithAll<NewlySelectedUnitTag>())
            {
                //UnityEngine.Debug.Log("Unit just selected");
                this.SpawnMarker(
                    ref state
                    , in physicsWorld
                    , in targetPosMarkerPrefab.Value
                    , currentWaypointRef.ValueRO.Value
                    , ref targetPosMarkerHolderRef.ValueRW);

            }

            foreach (var targetPosMarkerHolderRef in
                SystemAPI.Query<
                    RefRW<TargetPosMarkerHolder>>()
                    .WithAll<NewlyDeselectedUnitTag>())
            {
                //UnityEngine.Debug.Log("Unit just deselected");
                this.DestroyMarker(
                    in ecb
                    , ref targetPosMarkerHolderRef.ValueRW);

            }

        }

        [BurstCompile]
        private void SpawnMarker(
            ref SystemState state
            , in PhysicsWorldSingleton physicsWorld
            , in Entity markerPrefab
            , float3 rawPos
            , ref TargetPosMarkerHolder targetPosMarkerHolder)
        {

            rawPos.y = 100f;

            bool hit = this.CastRay(
                in physicsWorld
                , rawPos
                , out var raycastHit);

            if (!hit) return;

            float3 spawnPos = raycastHit.Position.Add(y: 0.05f);
            var markerEntity = state.EntityManager.Instantiate(markerPrefab);

            var transformRef = SystemAPI.GetComponentRW<LocalTransform>(markerEntity);
            transformRef.ValueRW.Position = spawnPos;

            targetPosMarkerHolder.Value = markerEntity;

        }

        [BurstCompile]
        private bool CastRay(
            in PhysicsWorldSingleton physicsWorld
            , float3 startPos
            , out Unity.Physics.RaycastHit raycastHit)
        {
            float3 rayStart = startPos;
            float3 rayEnd = startPos.Add(y: -500f);

            RaycastInput raycastInput = new()
            {
                Start = rayStart,
                End = rayEnd,
                Filter = new CollisionFilter
                {
                    BelongsTo = (uint) CollisionLayer.Ground,
                    CollidesWith = (uint) CollisionLayer.Ground,
                },
            };

            return physicsWorld.CastRay(raycastInput, out raycastHit);
        }

        [BurstCompile]
        private void DestroyMarker(
            in EntityCommandBuffer ecb
            , ref TargetPosMarkerHolder targetPosMarkerHolder)
        {
            ecb.DestroyEntity(targetPosMarkerHolder.Value);
            targetPosMarkerHolder.Value = Entity.Null;
        }

    }

}