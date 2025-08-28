using UnityEngine;

namespace Core.UI.InteractableActionsPanel.ActionPanel.ShowBuildModeActionPanel
{
    public class SendShowBuildModeMessageButton : BaseButton
    {
        [SerializeField] private ShowBuildModeActionPanelCtrl ctrl;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadCtrl(out this.ctrl);
        }

        private void LoadCtrl(out ShowBuildModeActionPanelCtrl ctrl) => ctrl = GetComponent<ShowBuildModeActionPanelCtrl>();

        protected override void OnClick() => this.ctrl.Activate();

    }

}