using Unity.Entities;
using UnityEngine;
using Components.Unit;
using Core;
using Components;

namespace Systems.Simulation.Unit
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(RaycastHitSelectionSystem))]
    public partial class SelectUnitSystem : SystemBase // This can be turned into ISystem when custom InputSystem is created.
    {

        protected override void OnCreate()
        {
            this.RequireForUpdate<SelectionHitData>();
            this.RequireForUpdate<SelectedUnitElement>();
            this.CreateUnitsHolder();
        }

        protected override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                this.ClearSelectedUnitsBuffer();
                return;
            }

            var selectionHitRef = SystemAPI.GetSingletonRW<SelectionHitData>();
            if (!selectionHitRef.ValueRO.NewlyAdded) return;
            if (selectionHitRef.ValueRO.SelectionType != SelectionType.Unit) return;
            selectionHitRef.ValueRW.NewlyAdded = false;

            this.AddUnitIntoHolder(selectionHitRef.ValueRO.RaycastHit.Entity);

        }

        private void AddUnitIntoHolder(in Entity hitEntity)
        {
            var selectedUnits = SystemAPI.GetSingletonBuffer<SelectedUnitElement>();

            for (int i = 0; i < selectedUnits.Length; i++)
            {
                if (selectedUnits.ElementAt(i).Value == hitEntity) return;
            }

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

    }
}