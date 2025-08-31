using Core.UI.MyImage;
using Core.UI.TextMeshProUGUIs;
using UnityEngine;

namespace Core.UI.InteractableActionsPanel.ActionPanel.RecruitActionPanel.RecruitActionCostView
{
    [GenerateUIType("RecruitActionCostView")]
    public partial class RecruitActionCostViewCtrl : BaseUICtrl
    {
        [SerializeField] private BaseImage resourceBGImage;
        [SerializeField] private BaseTextMeshProUGUI costText;

        public BaseImage ResourceBGImage => resourceBGImage;
        public BaseTextMeshProUGUI CostText => costText;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.resourceBGImage);
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