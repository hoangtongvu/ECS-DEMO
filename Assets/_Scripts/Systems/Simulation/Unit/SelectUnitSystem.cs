using Unity.Entities;
using Components.Unit;
using Core;
using Components;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

namespace Systems.Simulation.Unit
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(DragSelectionSystem))]
    [BurstCompile]
    public partial struct SelectUnitSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SelectionHitElement>();
            state.RequireForUpdate<SelectedUnitElement>();
            this.CreateUnitsHolder(ref state);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            var inputData = SystemAPI.GetSingleton<InputData>();
            if (inputData.BackspaceButtonDown)
            {
                this.ClearSelectedUnitsBuffer(ref state);
                return;
            }


            var selectionHits = SystemAPI.GetSingletonBuffer<SelectionHitElement>();
            if (selectionHits.IsEmpty) return;

            for (int i = 0; i < selectionHits.Length; i++)
            {
                var hit = selectionHits[i];
                if (hit.SelectionType != SelectionType.Unit) continue;

                this.AddUnitIntoHolder(ref state, hit.HitEntity);
                selectionHits.RemoveAt(i);
                i--;
            }


        }

        [BurstCompile]
        private void AddUnitIntoHolder(ref SystemState state, in Entity hitEntity)
        {
            var selectedUnits = SystemAPI.GetSingletonBuffer<SelectedUnitElement>();

            for (int i = 0; i < selectedUnits.Length; i++)
            {
                if (selectedUnits.ElementAt(i).Value == hitEntity) return;
            }

            // Set UnitSelected = true.
            var unitSelectedRef = SystemAPI.GetComponentRW<UnitSelected>(hitEntity);
            unitSelectedRef.ValueRW.Value = true;

            selectedUnits.Add(new SelectedUnitElement
            {
                Value = hitEntity,
            });
        }

        [BurstCompile]
        private void ClearSelectedUnitsBuffer(ref SystemState state)
        {
            var selectedUnits = SystemAPI.GetSingletonBuffer<SelectedUnitElement>();
            if (selectedUnits.IsEmpty) return;

            // Set false UnitSelected for all Units in DynamicBuffer.
            var job = new DisableUnitSelectedJob
            {
                SelectedUnits = selectedUnits.ToNativeArray(state.WorldUpdateAllocator),
                UnitSelectedLookup = SystemAPI.GetComponentLookup<UnitSelected>(),
            };

            state.Dependency = job.ScheduleParallel(selectedUnits.Length, 32, state.Dependency);

            selectedUnits.Clear();
        }

        [BurstCompile]
        private void CreateUnitsHolder(ref SystemState state)
        {
            SingletonUtilities.GetInstance(state.EntityManager)
                .AddBuffer<SelectedUnitElement>();
        }

        [BurstCompile]
        private struct DisableUnitSelectedJob : IJobParallelForBatch
        {
            public NativeArray<SelectedUnitElement> SelectedUnits;

            [NativeDisableParallelForRestriction]
            public ComponentLookup<UnitSelected> UnitSelectedLookup;

            public void Execute(int startIndex, int count)
            {
                int length = startIndex + count;
                for (int i = startIndex; i < length; i++)
                {
                    SelectedUnitElement unit = this.SelectedUnits[i];
                    var unitSelectedRef = this.UnitSelectedLookup.GetRefRWOptional(unit.Value);
                    unitSelectedRef.ValueRW.Value = false;
                }
            }
        }

    }
}