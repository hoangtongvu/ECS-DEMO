using Components.GameEntity.InteractableActions;
using Unity.Entities;
using UnityEngine;

namespace Systems.Simulation.GameEntity.InteractableActions
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