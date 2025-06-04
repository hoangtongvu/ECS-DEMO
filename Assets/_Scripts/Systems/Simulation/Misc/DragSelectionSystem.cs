using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using Utilities;
using Unity.Mathematics;
using Unity.Collections;
using Components.Unit;
using Components.Camera;
using Components.Misc;
using Core.Misc;

namespace Systems.Simulation.Misc
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(RaycastHitSelectionSystem))]
    public partial class DragSelectionSystem : SystemBase
    {
        private UnityEngine.Camera mainCamera;

        protected override void OnCreate()
        {
            this.CreateDragSelectionData();
            this.RequireForUpdate<MainCamHolder>();
        }

        protected override void OnStartRunning()
        {
            this.mainCamera = SystemAPI.GetSingleton<MainCamHolder>().Value;
        }

        protected override void OnUpdate()
        {
            var inputData = SystemAPI.GetSingleton<InputData>();
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var dragSelectionDataRef = SystemAPI.GetSingletonRW<DragSelectionData>();

            if (inputData.LeftMouseData.Down)
                this.OnInputDown(physicsWorld, dragSelectionDataRef);

            if (inputData.LeftMouseData.Hold)
                this.OnInputHold(physicsWorld, dragSelectionDataRef);

            if (inputData.LeftMouseData.Up)
                this.OnInputUp(physicsWorld, dragSelectionDataRef);

        }

        private bool CastRayToGround(in PhysicsWorldSingleton physicsWorld, out Unity.Physics.RaycastHit raycastHit)
        {
            UnityEngine.Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            float3 rayStart = ray.origin;
            float3 rayEnd = ray.GetPoint(100f);

            RaycastInput raycastInput = new()
            {
                Start = rayStart,
                End = rayEnd,
                Filter = new CollisionFilter
                {
                    BelongsTo = (uint) CollisionLayer.Default,
                    CollidesWith = (uint) CollisionLayer.Ground,
                }
            };

            return physicsWorld.CastRay(raycastInput, out raycastHit);
        }

        private void CreateDragSelectionData()
        {
            SingletonUtilities.GetInstance(EntityManager)
                .AddOrSetComponentData(new DragSelectionData
            {
                IsDragging = false,
                DistanceToConsiderDrag = 0.5f,
            });
        }

        private void OnInputDown(PhysicsWorldSingleton physicsWorld, RefRW<DragSelectionData> dragSelectionDataRef)
        {
            this.CastRayToGround(physicsWorld, out var raycastHit);
            dragSelectionDataRef.ValueRW.StartWorldPos = raycastHit.Position;
        }

        private void OnInputHold(PhysicsWorldSingleton physicsWorld, RefRW<DragSelectionData> dragSelectionDataRef)
        {
            this.CastRayToGround(physicsWorld, out var raycastHit);
            float3 currentPos = raycastHit.Position;
            float3 startPos = dragSelectionDataRef.ValueRO.StartWorldPos;

            if (this.MetDragDistance(
                startPos
                , currentPos
                , dragSelectionDataRef.ValueRO.DistanceToConsiderDrag))
            {
                dragSelectionDataRef.ValueRW.CurrentWorldPos = currentPos;
                dragSelectionDataRef.ValueRW.IsDragging = true;
            }
            else dragSelectionDataRef.ValueRW.IsDragging = false;
        }

        private void OnInputUp(PhysicsWorldSingleton physicsWorld, RefRW<DragSelectionData> dragSelectionDataRef)
        {
            if (!dragSelectionDataRef.ValueRO.IsDragging) return;
            dragSelectionDataRef.ValueRW.IsDragging = false;

            var selectionHits = SystemAPI.GetSingletonBuffer<SelectionHitElement>();


            float3 startPos = dragSelectionDataRef.ValueRO.StartWorldPos;
            float3 currentPos = dragSelectionDataRef.ValueRO.CurrentWorldPos;

            float3 centerPos = math.lerp(startPos, currentPos, 0.5f);
            float2 size2 =
                    new float2(startPos.x, startPos.z) -
                    new float2(currentPos.x, currentPos.z);

            float3 size3 = math.abs(new float3(size2.x, 50f, size2.y));

            NativeList<DistanceHit> distanceHits = new(Allocator.Temp);

            physicsWorld.OverlapBox(
                centerPos
                , quaternion.identity
                , size3 / 2
                , ref distanceHits
                , CollisionFilter.Default);

            foreach (var hit in distanceHits)
            {
                if (!SystemAPI.HasComponent<SelectableUnitTag>(hit.Entity)) continue;

                selectionHits.Add(new SelectionHitElement
                {
                    SelectionType = SelectionType.Unit,
                    HitEntity = hit.Entity,
                });
            }

            distanceHits.Dispose();

        }

        private bool MetDragDistance(
            float3 startPos
            , float3 currentPos
            , float distanceToConsiderDrag) => math.distance(startPos, currentPos) > distanceToConsiderDrag;

    }

}