using UnityEngine;

namespace Core.UI.InteractableActionsPanel.ActionPanel.RecruitActionPanel
{
    public class SendRecruitMessageButton : BaseButton
    {
        [SerializeField] private RecruitActionPanelCtrl ctrl;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadCtrl(out this.ctrl);
        }

        private void LoadCtrl(out RecruitActionPanelCtrl ctrl) => ctrl = GetComponent<RecruitActionPanelCtrl>();

        protected override void OnClick() => this.ctrl.Activate();

    }

}