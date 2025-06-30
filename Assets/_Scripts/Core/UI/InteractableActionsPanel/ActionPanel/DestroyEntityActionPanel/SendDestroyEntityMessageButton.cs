using UnityEngine;

namespace Core.UI.InteractableActionsPanel.ActionPanel.DestroyEntityActionPanel
{
    public class SendDestroyEntityMessageButton : BaseButton
    {
        [SerializeField] private DestroyEntityActionPanelCtrl ctrl;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadCtrl(out this.ctrl);
        }

        private void LoadCtrl(out DestroyEntityActionPanelCtrl ctrl) => ctrl = GetComponent<DestroyEntityActionPanelCtrl>();

        protected override void OnClick() => this.ctrl.Activate();

    }

}