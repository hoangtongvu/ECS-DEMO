using Core.MyEvent.PubSub.Messengers;
using Core.Unit.Misc;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.UI.InteractableActionsPanel.ActionPanel.RecruitActionPanel
{
    [GenerateUIType("ActionPanel_Recruit")]
    public partial class RecruitActionPanelCtrl : ActionPanelCtrl
    {
        [SerializeField]
        private RecruitActionCostViewsHolder recruitActionCostViewsHolder;

        public RecruitActionCostViewsHolder RecruitActionCostViewsHolder => recruitActionCostViewsHolder;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.recruitActionCostViewsHolder);
        }

        public override void Activate()
        {
            GameplayMessenger.MessagePublisher
                .Publish(new RecruitUnitMessage(this.BaseEntity));
        }

        public override void OnRent() { }

        public override void OnReturn()
        {
            foreach (var recruitActionCostView in this.recruitActionCostViewsHolder.Value)
            {
                recruitActionCostView.ReturnSelfToPool();
            }

            this.recruitActionCostViewsHolder.Value.Clear();
        }

    }

}