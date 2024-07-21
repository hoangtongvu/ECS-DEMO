using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using Utilities;
using Unity.Mathematics;
using Components;
using Components.Unit;
using Core;
using Components.Camera;
using Components.Harvest;

namespace Systems.Simulation
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class RaycastHitSelectionSystem : SystemBase
    {
        private UnityEngine.Camera mainCamera;


        protected override void OnCreate()
        {
            this.CreateSelectionHitsHolder();

            this.RequireForUpdate<SelectionHitElement>();
            this.RequireForUpdate<SelectableUnitTag>();
            this.RequireForUpdate<MainCamHolder>();
        }

        protected override void OnStartRunning()
        {
            this.mainCamera = SystemAPI.GetSingleton<MainCamHolder>().Value;
        }

        protected override void OnUpdate()
        {
            this.ClearSelectionHits();

            var inputData = SystemAPI.GetSingleton<InputData>();
            if (!inputData.RightMouseData.Up) return;

            PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

            this.CastRay(physicsWorld, out var raycastHit);
            this.AddNewSelectionHit(raycastHit);
        }

        private void AddNewSelectionHit(in Unity.Physics.RaycastHit raycastHit)
        {
            var selectionHits = SystemAPI.GetSingletonBuffer<SelectionHitElement>();

            selectionHits.Add(new SelectionHitElement
            {
                SelectionType = this.GetSelectionType(raycastHit),
                HitEntity = raycastHit.Entity,
                HitPos = raycastHit.Position,
            });
        }

        private void ClearSelectionHits()
        {
            var selectionHits = SystemAPI.GetSingletonBuffer<SelectionHitElement>();
            selectionHits.Clear();
        }


        private void CreateSelectionHitsHolder()
        {
            SingletonUtilities.GetInstance(EntityManager)
                .AddBuffer<SelectionHitElement>();
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
                Filter = CollisionFilter.Default, // this means all entities can be catched.
            };

            return physicsWorld.CastRay(raycastInput, out raycastHit);
        }

        private SelectionType GetSelectionType(in Unity.Physics.RaycastHit raycastHit)
        {
            Entity hitEntity = raycastHit.Entity;

            if (SystemAPI.HasComponent<HarvesteeTag>(hitEntity))
            {
                return SelectionType.Harvestee;
            }

            if (!SystemAPI.HasComponent<SelectableUnitTag>(hitEntity))
            {
                // if (!SystemAPI.HasComponent<GroundTag>(hitEntity)) return;
                return SelectionType.Position;
            }
            return SelectionType.Unit;
        }

    }
}