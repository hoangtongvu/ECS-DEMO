using Core.GameResource;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.TopLeftPanel.ResourceDisplay
{
    [GenerateUIType("ResourceDisplay")]
    public partial class ResourceDisplayCtrl : BaseUICtrl
    {
        [SerializeField] private Image backGroundImage;
        [SerializeField] private QuantityText quantityText;

        public ResourceType ResourceType;

        public QuantityText QuantityText => quantityText;
        public Image BackGroundImage => backGroundImage;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.quantityText);
            this.LoadComponentInChildren(ref this.backGroundImage);
        }

        public override void OnRent()
        {
        }

        public override void OnReturn()
        {
        }

    }

}