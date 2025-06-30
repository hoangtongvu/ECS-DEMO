using UnityEngine;

namespace Core.UI.InteractableActionsPanel.ActionPanel.EntitySpawningProfileActionPanel
{
    public class SendSpawnEntityMessageButton : BaseButton
    {
        [SerializeField] private EntitySpawningProfileActionPanelCtrl ctrl;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadCtrl(out this.ctrl);
        }

        private void LoadCtrl(out EntitySpawningProfileActionPanelCtrl ctrl) => ctrl = GetComponent<EntitySpawningProfileActionPanelCtrl>();

        protected override void OnClick() => this.ctrl.Activate();

    }

}