using Core.UI.InteractableActionsPanel.ActionPanel;
using UnityEngine;

namespace Core.UI.InteractableActionsPanel
{
    [GenerateUIType("ActionsContainerUI")]
    public partial class ActionsContainerUICtrl : BaseUICtrl, IReusableUI
    {
        [SerializeField] private ActionPanelsHolder actionPanelsHolder;
        [SerializeField] private ActionPanelCtrl chosenActionPanelCtrl;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private ActionsContainerUITweener tweener;

        public CanvasGroup CanvasGroup => canvasGroup;
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
            this.LoadComponentInChildren(out this.actionPanelsHolder);
            this.LoadComponentInCtrl(out this.canvasGroup);
            this.tweener = new() { ctrl = this };
        }

        public override void OnRent()
        {
            this.tweener.TriggerTweenOnAppear();
            this.chosenActionPanelCtrl = null;
        }

        public override void TriggerHiding() => this.tweener.TriggerTweenOnDisappear();

        public override void OnReturn()
        {
            this.ClearChildren();
        }

        public void Reuse()
        {
            this.tweener.TriggerTweenOnAppear();
            this.chosenActionPanelCtrl = null;
        }

        private void ClearChildren()
        {
            foreach (var actionPanel in this.actionPanelsHolder.Value)
            {
                actionPanel.ReturnSelfToPool();
            }

            this.actionPanelsHolder.Value.Clear();
        }

    }

}