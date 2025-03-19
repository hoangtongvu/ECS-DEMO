using Core.GameResource;
using UnityEngine;

namespace Core.UI.TopLeftPanel.ResourceDisplay
{
    [GenerateUIType("ResourceDisplay")]
    public partial class ResourceDisplayCtrl : BaseUICtrl
    {
        [SerializeField] private ResourceImage resourceImage;
        [SerializeField] private QuantityText quantityText;

        public ResourceType ResourceType;

        public ResourceImage ResourceImage => resourceImage;
        public QuantityText QuantityText => quantityText;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.resourceImage);
            this.LoadComponentInChildren(ref this.quantityText);
        }

    }

}