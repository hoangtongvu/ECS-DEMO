using Unity.Entities;
using Core;
using Components;
using Unity.Burst;
using Components.Unit.UnitSelection;

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
            state.RequireForUpdate<UnitSelectedTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            var inputData = SystemAPI.GetSingleton<InputData>();
            if (inputData.BackspaceButtonDown)
            {
                this.DisableUnitSelectedTag(ref state);
                return;
            }


            var selectionHits = SystemAPI.GetSingletonBuffer<SelectionHitElement>();
            if (selectionHits.IsEmpty) return;

            for (int i = 0; i < selectionHits.Length; i++)
            {
                var hit = selectionHits[i];
                if (hit.SelectionType != SelectionType.Unit) continue;

                this.EnableUnitSelectedTag(ref state, hit.HitEntity);
                selectionHits.RemoveAt(i);
                i--;
            }


        }

        [BurstCompile]
        private void EnableUnitSelectedTag(ref SystemState state, in Entity hitEntity)
            => SystemAPI.SetComponentEnabled<UnitSelectedTag>(hitEntity, true);

        [BurstCompile]
        private void DisableUnitSelectedTag(ref SystemState state)
        {
            foreach (var unitSelectedTag in
                SystemAPI.Query<EnabledRefRW<UnitSelectedTag>>())
            {
                unitSelectedTag.ValueRW = false;
            }
        }


    }
}