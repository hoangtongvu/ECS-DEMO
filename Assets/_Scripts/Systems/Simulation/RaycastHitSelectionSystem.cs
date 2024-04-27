using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using Unity.Mathematics;
using Components;
using Components.Unit;
using Core;

namespace Systems.Simulation
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class RaycastHitSelectionSystem : SystemBase
    {
        private Camera mainCamera;


        protected override void OnCreate()
        {
            this.mainCamera = CameraHolderCtrl.Instance.MainCam;
            this.CreateSelectionHitsHolder();

            this.RequireForUpdate<SelectionHitElement>();
            this.RequireForUpdate<SelectableUnitTag>();
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
                RaycastHit = raycastHit,
            });
        }

        private void ClearSelectionHits()
        {
            var selectionHits = SystemAPI.GetSingletonBuffer<SelectionHitElement>();
            selectionHits.Clear();
        }


        private void CreateSelectionHitsHolder()
        {
            Entity entity = EntityManager.CreateEntity();

            EntityManager.AddBuffer<SelectionHitElement>(entity);
            EntityManager.SetName(entity, "SelectionHitsHolder");
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

            if (!SystemAPI.HasComponent<SelectableUnitTag>(hitEntity))
            {
                // if (!SystemAPI.HasComponent<GroundTag>(hitEntity)) return;
                return SelectionType.Position;
            }
            return SelectionType.Unit;
        }

    }
}