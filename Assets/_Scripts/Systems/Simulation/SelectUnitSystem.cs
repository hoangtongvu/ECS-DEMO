using Components;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using Unity.Mathematics;

namespace Systems.Simulation
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class SelectUnitSystem : SystemBase
    {
        private Camera mainCamera;


        protected override void OnCreate()
        {
            this.mainCamera = CameraHolderCtrl.Instance.MainCam;
            RequireForUpdate<SelectableUnitTag>();
            RequireForUpdate<SelectedState>();
        }

        protected override void OnUpdate()
        {
            if (!Input.GetMouseButtonDown(1)) return;
            PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            this.SelectSingleUnit(physicsWorld);
        }

        private void SelectSingleUnit(in PhysicsWorldSingleton physicsWorld)
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

            if (physicsWorld.CastRay(raycastInput, out var raycastHit))
            {
                Entity hitEntity = raycastHit.Entity;
                if (!SystemAPI.HasComponent<SelectableUnitTag>(hitEntity)) return;
                RefRW<SelectedState> selectedStateRef = SystemAPI.GetComponentRW<SelectedState>(hitEntity);
                this.ToggleSelectedState(ref selectedStateRef.ValueRW);
            }
        }

        private void ToggleSelectedState(ref SelectedState selectedState) => selectedState.Value = !selectedState.Value;


    }
}