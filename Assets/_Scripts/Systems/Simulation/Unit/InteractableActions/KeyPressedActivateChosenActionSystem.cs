using Components.Unit.InteractableActions;
using Unity.Entities;
using UnityEngine;

namespace Systems.Simulation.Unit.InteractableActions
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class KeyPressedActivateChosenActionSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    ActionsContainerUIHolder>()
                .Build();

            this.RequireForUpdate(query0);
        }

        protected override void OnUpdate()
        {
            if (!Input.GetKeyDown(KeyCode.E)) return;

            foreach (var actionsContainerUIHolderRef in SystemAPI
                .Query<
                    RefRO<ActionsContainerUIHolder>>())
            {
                actionsContainerUIHolderRef.ValueRO.Value.Value?.ChosenActionPanelCtrl?.Activate();
            }

        }

    }

}