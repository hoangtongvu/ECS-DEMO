using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using Unity.Mathematics;
using Components.Unit;
using Unity.Burst.CompilerServices;

namespace Systems.Simulation.Unit
{

    public enum SelectionType
    {
        Position = 0,
        Unit = 1,
        UI = 2,

    }

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class SelectUnitSystem : SystemBase
    {
        private Camera mainCamera;


        protected override void OnCreate()
        {
            this.mainCamera = CameraHolderCtrl.Instance.MainCam;
            RequireForUpdate<SelectableUnitTag>();
            this.CreateUnitsHolder();
        }

        protected override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                this.ClearSelectedUnitsBuffer();
                return;
            }

            if (!Input.GetMouseButtonDown(1)) return;
            PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            this.SelectSingleUnit(physicsWorld);
        }

        // TODO: Casting a ray is a general logic that return Entity. Move this logic to a static function.
        private void SelectSingleUnit(in PhysicsWorldSingleton physicsWorld) 
        {
            
            if (!this.CastRay(physicsWorld, out var raycastHit)) return;

            SelectionType selectionType = this.GetSelectionType(raycastHit);

            switch (selectionType)
            {
                case SelectionType.Position:
                    var selectedUnits = SystemAPI.GetSingletonBuffer<SelectedUnitElement>();
                    foreach (var unit in selectedUnits)
                    {
                        if (!SystemAPI.HasComponent<MoveableState>(unit.Value)) continue;

                        if (Hint.Likely(!SystemAPI.IsComponentEnabled<MoveableState>(unit.Value)))
                        {
                            SystemAPI.SetComponentEnabled<MoveableState>(unit.Value, true);
                        }

                        RefRW<TargetPosition> targetPosRef = SystemAPI.GetComponentRW<TargetPosition>(unit.Value);
                        targetPosRef.ValueRW.Value = raycastHit.Position;
                    }
                    break;
                case SelectionType.Unit:
                    this.AddUnitIntoHolder(raycastHit.Entity);
                    break;
                case SelectionType.UI:
                    
                    break;

            }

        }


        private void AddUnitIntoHolder(in Entity hitEntity)
        {
            var selectedUnits = SystemAPI.GetSingletonBuffer<SelectedUnitElement>();
            selectedUnits.Add(new SelectedUnitElement
            {
                Value = hitEntity,
            });
        }

        private void ClearSelectedUnitsBuffer()
        {
            var selectedUnits = SystemAPI.GetSingletonBuffer<SelectedUnitElement>();
            selectedUnits.Clear();
        }

        private void CreateUnitsHolder()
        {
            Entity unitsHolder = EntityManager.CreateEntity();

            EntityManager.AddBuffer<SelectedUnitElement>(unitsHolder);
            EntityManager.SetName(unitsHolder, "SelectedUnitsHolder");
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

    }
}