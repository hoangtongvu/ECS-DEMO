using Core.UI.TextMeshProUGUIs;
using UnityEngine;

namespace Core.UI.TopLeftPanel.ResourceDisplay
{
    public class QuantityText : BaseTextMeshProUGUI
    {
        [SerializeField] private ResourceDisplayCtrl resourceDisplayCtrl;
        [SerializeField] private QuantityTextTweener quantityTextTweener = new();

        protected override void Awake()
        {
            base.Awake();
            this.quantityTextTweener.QuantityText = this;
            this.quantityTextTweener.OriginalTextColor = this.text.color;
        }

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadCtrl(ref this.resourceDisplayCtrl);
        }

        private void OnEnable()
        {
            this.resourceDisplayCtrl.ResourceDisplayEventHandler.OnQuantityChanged += this.UpdateQuantityFromDataPool;
            this.resourceDisplayCtrl.ResourceDisplayEventHandler.OnQuantityAdded += this.quantityTextTweener.TriggerOnQuantityAddedTweens;
            this.resourceDisplayCtrl.ResourceDisplayEventHandler.OnQuantityDeducted += this.quantityTextTweener.TriggerOnQuantityDeductedTweens;
        }

        private void OnDisable()
        {
            this.resourceDisplayCtrl.ResourceDisplayEventHandler.OnQuantityChanged -= this.UpdateQuantityFromDataPool;
            this.resourceDisplayCtrl.ResourceDisplayEventHandler.OnQuantityAdded -= this.quantityTextTweener.TriggerOnQuantityAddedTweens;
            this.resourceDisplayCtrl.ResourceDisplayEventHandler.OnQuantityDeducted -= this.quantityTextTweener.TriggerOnQuantityDeductedTweens;
        }

        private void UpdateQuantityFromDataPool()
        {
            uint quantity = this.resourceDisplayCtrl.ResourceDisplayData.ResourceQuantity;
            this.SetQuantity(quantity);
        }

        private void SetQuantity(uint quantity) => this.text.text = $"{quantity}";

    }
}