using Components.GameEntity.Damage;
using Components.GameEntity.Misc;
using Components.GameEntity.Movement;
using Components.MyCamera;
using Core.Misc;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Systems.Simulation.GameEntity.Misc
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(RotateEntityToLookDirSystem))]
    [UpdateAfter(typeof(SetLookDirToMoveDirSystem))]
    public partial class SetLookDirToAttackDirSystem : SystemBase
    {
        private UnityEngine.Camera mainCamera;

        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    LookDirectionXZ
                    , MoveDirectionFloat2>()
                .WithAll<
                    CanMoveEntityTag>()
                .WithAll<
                    IsAlive>()
                .Build();

            this.RequireForUpdate(query0);
            this.RequireForUpdate<MainCamHolder>();
        }

        protected override void OnStartRunning()
        {
            this.mainCamera = SystemAPI.GetSingleton<MainCamHolder>().Value;
        }

        protected override void OnUpdate()
        {
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

            foreach (var (lookDirXZRef, transformRef) in SystemAPI
                .Query<
                    RefRW<LookDirectionXZ>
                    , RefRO<LocalTransform>>()
                .WithAll<
                    InAttackStateTag>()
                .WithAll<
                    IsAlive>())
            {
                this.CastRay(in physicsWorld, out var raycastHit);

                float3 tempVector3 = raycastHit.Position - transformRef.ValueRO.Position;
                float2 tempVector2 = new(tempVector3.x, tempVector3.z);

                lookDirXZRef.ValueRW.Value = math.normalize(tempVector2);
            }

        }

        private bool CastRay(in PhysicsWorldSingleton physicsWorld, out Unity.Physics.RaycastHit raycastHit)
        {
            UnityEngine.Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            float3 rayStart = ray.origin;
            float3 rayEnd = ray.GetPoint(100f);

            RaycastInput raycastInput = new()
            {
                Start = rayStart,
                End = rayEnd,
                Filter = new()
                {
                    BelongsTo = (uint)CollisionLayer.Default,
                    CollidesWith = (uint)CollisionLayer.Ground,
                }
            };

            return physicsWorld.CastRay(raycastInput, out raycastHit);
        }

    }

}