using Unity.Entities;
using Components.Unit.UnitSelection;
using Utilities.Helpers;
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
    public partial class TargetPosMarkerUISystem : SystemBase
    {

        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    TargetPosition
                    , UnitTargetPosUIID
                    , NewlySelectedUnitTag
                    , NewlyDeselectedUnitTag>()
                .Build();

            this.RequireForUpdate(query);
        }


        protected override void OnUpdate()
        {
            var spawnedUIMap = SystemAPI.ManagedAPI.GetSingleton<SpawnedUIMap>();
            var uiPoolMap = SystemAPI.ManagedAPI.GetSingleton<UIPoolMap>();
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

            foreach (var (targetPosRef, markerUIIDRef) in
                SystemAPI.Query<
                    RefRO<TargetPosition>
                    , RefRW<UnitTargetPosUIID>>()
                    .WithAll<NewlySelectedUnitTag>())
            {
                //UnityEngine.Debug.Log("Unit just selected");

                this.ShowMarker(
                    in physicsWorld
                    , spawnedUIMap
                    , uiPoolMap
                    , UIType.UnitTargetPosMark
                    , targetPosRef.ValueRO.Value
                    , ref markerUIIDRef.ValueRW);

            }

            foreach (var markerUIIDRef in
                SystemAPI.Query<
                    RefRW<UnitTargetPosUIID>>()
                    .WithAll<NewlyDeselectedUnitTag>())
            {
                //UnityEngine.Debug.Log("Unit just deselected");

                this.HideMarker(
                    spawnedUIMap
                    , uiPoolMap
                    , ref markerUIIDRef.ValueRW);
            }

        }

        private void ShowMarker(
            in PhysicsWorldSingleton physicsWorld
            , SpawnedUIMap spawnedUIMap
            , UIPoolMap uiPoolMap
            , UIType uiType
            , float3 rawPos
            , ref UnitTargetPosUIID markerUIID)
        {
            // Take raw pos from TargetPos
            // set that raw pos's y = 100f
            // Shoot ray from new pos -> get pos on ground
            // add 0.05f to y of pos on ground -> it is the spawn pos
            
            rawPos.y = 100f;

            bool hit = this.CastRay(
                in physicsWorld
                , rawPos
                , out var raycastHit);

            if (!hit) return;

            float3 spawnPos = raycastHit.Position.Add(y: 0.05f);

            var baseUICtrl =
                UISpawningHelper.Spawn(
                    uiPoolMap
                    , spawnedUIMap
                    , uiType
                    , spawnPos);

            markerUIID.Value = baseUICtrl.UIID;
            baseUICtrl.gameObject.SetActive(true);

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


        private void HideMarker(
            SpawnedUIMap spawnedUIMap
            , UIPoolMap uiPoolMap
            , ref UnitTargetPosUIID markerUIID)
        {
            UISpawningHelper.Despawn(uiPoolMap, spawnedUIMap, markerUIID.Value);
            markerUIID.Value = default;
        }

    }
}