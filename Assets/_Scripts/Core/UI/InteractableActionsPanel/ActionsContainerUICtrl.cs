using Core.UI.Identification;
using Core.UI.InteractableActionsPanel.ActionPanel;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI.InteractableActionsPanel
{
    [GenerateUIType("ActionsContainerUI")]
    public partial class ActionsContainerUICtrl : BaseUICtrl
    {
        [SerializeField] private ActionPanelsHolder actionPanelsHolder;
        [SerializeField] private ActionPanelCtrl chosenActionPanelCtrl;

        public ActionPanelCtrl ChosenActionPanelCtrl
        {
            get => chosenActionPanelCtrl;
            set
            {
                this.chosenActionPanelCtrl?.OnBeingUnchosen();
                this.chosenActionPanelCtrl = value;
                this.chosenActionPanelCtrl?.OnBeingChosen();
            }
        }

        public ActionPanelsHolder ActionPanelsHolder => actionPanelsHolder;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.actionPanelsHolder);
        }

        public override void Despawn(Dictionary<UIType, UIPrefabAndPool> uiPrefabAndPoolMap, Dictionary<UIID, BaseUICtrl> spawnedUIMap)
        {
            base.Despawn(uiPrefabAndPoolMap, spawnedUIMap);

            foreach (var actionPanel in this.actionPanelsHolder.Value)
            {
                actionPanel.Despawn(uiPrefabAndPoolMap, spawnedUIMap);
            }

            this.actionPanelsHolder.Value.Clear();
            this.chosenActionPanelCtrl = null;

        }

    }

}