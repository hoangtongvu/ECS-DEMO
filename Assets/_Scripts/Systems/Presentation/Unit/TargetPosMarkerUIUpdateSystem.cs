using Unity.Entities;
using Components.Unit.UnitSelection;
using Components.ComponentMap;
using Core.UI.Identification;
using Unity.Mathematics;
using Components;
using Unity.Physics;
using Core.Utilities.Extensions;
using Core;
using Components.Unit;

namespace Systems.Presentation.Unit
{

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateAfter(typeof(TargetPosMarkerUISpawnSystem))]
    public partial class TargetPosMarkerUIUpdateSystem : SystemBase
    {

        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    TargetPosition
                    , UnitTargetPosUIID
                    , TargetPosChangedTag
                    , UnitSelectedTag>()
                .Build();

            this.RequireForUpdate(query);
        }


        protected override void OnUpdate()
        {
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

            foreach (var (targetPosRef, markerUIIDRef) in
                SystemAPI.Query<
                    RefRO<TargetPosition>
                    , RefRO<UnitTargetPosUIID>>()
                    .WithAll<TargetPosChangedTag>()
                    .WithAll<UnitSelectedTag>())
            {

                UIID uiID = markerUIIDRef.ValueRO.Value;
                bool foundIdInMap = spawnedUIMap.Value.TryGetValue(uiID, out var baseUICtrl);

                if (!foundIdInMap)
                {
                    UnityEngine.Debug.LogError($"SpawnedUIMap has no Element with id = {uiID}");
                }

                bool canGetSpawnPos =
                    this.TryGetSpawnPos(
                        in physicsWorld
                        , targetPosRef.ValueRO.Value
                        , out float3 spawnPos);

                if (!canGetSpawnPos) continue;

                baseUICtrl.transform.position = spawnPos;

            }


        }

        private bool TryGetSpawnPos(
            in PhysicsWorldSingleton physicsWorld
            , float3 rawPos
            , out float3 spawnPos)
        {
            spawnPos = float3.zero;
            rawPos.y = 100f;

            bool hit = this.CastRay(
                in physicsWorld
                , rawPos
                , out var raycastHit);

            if (!hit) return false;

            spawnPos = raycastHit.Position.Add(y: 0.05f);
            return true;
        }


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


    }
}