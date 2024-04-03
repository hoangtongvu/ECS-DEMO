using Unity.Entities;
using Components.Unit;
using Core;
using Components;
using Unity.Burst;

namespace Systems.Simulation.Unit
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(RaycastHitSelectionSystem))]
    [BurstCompile]
    public partial struct SelectUnitSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SelectionHitData>();
            state.RequireForUpdate<SelectedUnitElement>();
            this.CreateUnitsHolder(state);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            var inputData = SystemAPI.GetSingleton<InputData>();
            if (inputData.BackspaceButtonDown)
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

        [BurstCompile]
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

        [BurstCompile]
        private void ClearSelectedUnitsBuffer()
        {
            var selectedUnits = SystemAPI.GetSingletonBuffer<SelectedUnitElement>();
            selectedUnits.Clear();
        }

        [BurstCompile]
        private void CreateUnitsHolder(SystemState state)
        {
            EntityManager em = state.EntityManager;
            Entity unitsHolder = em.CreateEntity();

            em.AddBuffer<SelectedUnitElement>(unitsHolder);
            em.SetName(unitsHolder, "SelectedUnitsHolder");
        }

    }
}