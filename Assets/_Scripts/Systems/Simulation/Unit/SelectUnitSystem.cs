using Unity.Entities;
using Unity.Burst;
using Components.Unit.UnitSelection;
using Systems.Simulation.Misc;
using Components.Misc;
using Core.Misc;

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
            this.ClearNewlyTags(ref state);

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

                
                selectionHits.RemoveAt(i);
                i--;

                var hitEntity = hit.HitEntity;
                bool unitIsSelected = SystemAPI.IsComponentEnabled<UnitSelectedTag>(hitEntity);

                if (unitIsSelected) continue;
                this.EnableUnitSelectedTag(ref state, hitEntity);

            }

        }

        [BurstCompile]
        private void EnableUnitSelectedTag(ref SystemState state, in Entity hitEntity)
        {
            SystemAPI.SetComponentEnabled<UnitSelectedTag>(hitEntity, true);
            SystemAPI.SetComponentEnabled<NewlySelectedUnitTag>(hitEntity, true);
        }

        [BurstCompile]
        private void DisableUnitSelectedTag(ref SystemState state)
        {
            foreach (var (unitSelectedTag, entity) in
                SystemAPI.Query<
                    EnabledRefRW<UnitSelectedTag>>()
                    .WithEntityAccess())
            {
                unitSelectedTag.ValueRW = false;
                SystemAPI.SetComponentEnabled<NewlyDeselectedUnitTag>(entity, true);
            }

        }

        [BurstCompile]
        private void ClearNewlyTags(ref SystemState state)
        {
            foreach (var tag in SystemAPI.Query<EnabledRefRW<NewlySelectedUnitTag>>())
            {
                tag.ValueRW = false;
                //UnityEngine.Debug.Log("SelectedTag cleared");
            }

            foreach (var tag in SystemAPI.Query<EnabledRefRW<NewlyDeselectedUnitTag>>())
            {
                tag.ValueRW = false;
                //UnityEngine.Debug.Log("DeselectedTag cleared");
            }

        }

    }

}