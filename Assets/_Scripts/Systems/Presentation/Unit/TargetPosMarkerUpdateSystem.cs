using Unity.Entities;
using Components.Unit.UnitSelection;
using Unity.Mathematics;
using Unity.Physics;
using Core.Utilities.Extensions;
using Components.Unit;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;
using Components.GameEntity.Movement;
using Core.Misc;

namespace Systems.Presentation.Unit
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateAfter(typeof(TargetPosMarkerSpawnSystem))]
    [BurstCompile]
    public partial struct TargetPosMarkerUpdateSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    CurrentWorldWaypoint
                    , TargetPosChangedTag
                    , UnitSelectedTag>()
                .Build();

            state.RequireForUpdate(query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

            state.Dependency =
                new UpdateMarkerJob
                {
                    PhysicsWorld = physicsWorld,
                    TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(),
                }.ScheduleParallel(state.Dependency);

        }

        [WithAll(typeof(UnitSelectedTag))]
        [WithAll(typeof(TargetPosChangedTag))]
        [BurstCompile]
        private partial struct UpdateMarkerJob : IJobEntity
        {
            [ReadOnly]
            public PhysicsWorldSingleton PhysicsWorld;

            [NativeDisableParallelForRestriction]
            public ComponentLookup<LocalTransform> TransformLookup;

            [BurstCompile]
            void Execute(
                CurrentWorldWaypoint currentWaypoint
                , TargetPosMarkerHolder targetPosMarkerHolder)
            {
                bool canGetTargetMarkerPos =
                    this.TryGetTargetMarkerPos(
                        in PhysicsWorld
                        , currentWaypoint.Value
                        , out float3 targetMarkerPos);

                if (!canGetTargetMarkerPos) return;

                var transformRef = this.TransformLookup.GetRefRWOptional(targetPosMarkerHolder.Value);
                transformRef.ValueRW.Position = targetMarkerPos;

            }

            [BurstCompile]
            private bool TryGetTargetMarkerPos(
            in PhysicsWorldSingleton physicsWorld
            , float3 rawPos
            , out float3 targetMarkerPos)
            {
                targetMarkerPos = float3.zero;
                rawPos.y = 100f;

                bool hit = this.CastRay(
                    in physicsWorld
                    , rawPos
                    , out var raycastHit);

                if (!hit) return false;

                targetMarkerPos = raycastHit.Position.Add(y: 0.05f);
                return true;
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
                        BelongsTo = (uint)CollisionLayer.Ground,
                        CollidesWith = (uint)CollisionLayer.Ground,
                    },
                };

                return physicsWorld.CastRay(raycastInput, out raycastHit);
            }

        }

    }

}