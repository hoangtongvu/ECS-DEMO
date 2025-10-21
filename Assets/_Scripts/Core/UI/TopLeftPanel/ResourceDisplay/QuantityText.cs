using Core.UI.TextMeshProUGUIs;
using UnityEngine;

namespace Core.UI.TopLeftPanel.ResourceDisplay
{
    public class QuantityText : BaseTextMeshProUGUI
    {
        [SerializeField] private ResourceDisplayCtrl resourceDisplayCtrl;
        [SerializeField] private QuantityTextTweener quantityTextTweener = new();

        private void Awake()
        {
            this.quantityTextTweener.QuantityText = this;
            this.quantityTextTweener.OriginalTextColor = this.text.color;
        }

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadCtrl(out this.resourceDisplayCtrl);
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