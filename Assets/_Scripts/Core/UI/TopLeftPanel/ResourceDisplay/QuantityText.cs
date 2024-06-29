using Core.UI.LegacyText;
using UnityEngine;

namespace Core.UI.TopLeftPanel.ResourceDisplay
{
    public class QuantityText : BaseText
    {
        public void SetQuantity(uint quantity) => this.text.text = quantity.ToString();
    }
}