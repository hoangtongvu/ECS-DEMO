using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.TopLeftPanel.ResourceDisplay
{
    [GenerateUIType("ResourceDisplay")]
    public partial class ResourceDisplayCtrl : BaseUICtrl
    {
        private ResourceDisplayEventHandler resourceDisplayEventHandler;
        [SerializeField] private Image backGroundImage;
        [SerializeField] private QuantityText quantityText;
        [SerializeField] private ResourceDisplayTweener resourceDisplayTweener = new();

        public ResourceDisplayData ResourceDisplayData = new();

        public ResourceDisplayEventHandler ResourceDisplayEventHandler => resourceDisplayEventHandler;
        public QuantityText QuantityText => quantityText;
        public Image BackGroundImage => backGroundImage;

        protected override void Awake()
        {
            base.Awake();

            this.resourceDisplayTweener.ResourceDisplayCtrl = this;
            this.resourceDisplayEventHandler = new()
            {
                ResourceDisplayCtrl = this,
            };
        }

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.quantityText);
            this.LoadComponentInChildren(ref this.backGroundImage);
        }

        public override void OnRent()
        {
            this.resourceDisplayEventHandler.Bind();
            this.resourceDisplayEventHandler.OnQuantityChanged += this.resourceDisplayTweener.TriggerOnQuantityChangedTweens;
        }

        public override void OnReturn()
        {
            this.resourceDisplayEventHandler.UnBind();
            this.resourceDisplayEventHandler.OnQuantityChanged -= this.resourceDisplayTweener.TriggerOnQuantityChangedTweens;
        }

        private void OnDestroy()
        {
            this.resourceDisplayEventHandler.UnBind();
            this.resourceDisplayEventHandler.OnQuantityChanged -= this.resourceDisplayTweener.TriggerOnQuantityChangedTweens;
        }

    }

}