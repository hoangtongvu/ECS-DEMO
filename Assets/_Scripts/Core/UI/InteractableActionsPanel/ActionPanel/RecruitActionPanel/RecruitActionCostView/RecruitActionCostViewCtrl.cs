using Core.UI.MyImage;
using Core.UI.TextMeshProUGUIs;
using UnityEngine;

namespace Core.UI.InteractableActionsPanel.ActionPanel.RecruitActionPanel.RecruitActionCostView
{
    [GenerateUIType("RecruitActionCostView")]
    public partial class RecruitActionCostViewCtrl : BaseUICtrl
    {
        [SerializeField] private BaseImage resourceIcon;
        [SerializeField] private BaseTextMeshProUGUI costText;

        public BaseImage ResourceIcon => resourceIcon;
        public BaseTextMeshProUGUI CostText => costText;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.resourceIcon);
            this.LoadComponentInChildren(ref this.costText);
        }

        public override void OnRent()
        {
        }

        public override void OnReturn()
        {
        }

    }

}