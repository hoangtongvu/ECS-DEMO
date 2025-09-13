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
            this.RequireForUpdate<ActionsContainerUI_CD.Holder>();
        }

        protected override void OnUpdate()
        {
            if (!Input.GetKeyDown(KeyCode.E)) return;

            var actionsContainerUICtrl = SystemAPI.GetSingleton<ActionsContainerUI_CD.Holder>().Value.Value;
            actionsContainerUICtrl?.ChosenActionPanelCtrl?.Activate();
        }

    }

}