using Core.UI.InteractableActionsPanel.ActionPanel;
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

        public override void OnRent() { }

        public override void OnReturn()
        {
            foreach (var actionPanel in this.actionPanelsHolder.Value)
            {
                actionPanel.ReturnSelfToPool();
            }

            this.actionPanelsHolder.Value.Clear();
            this.chosenActionPanelCtrl = null;
        }

    }

}