using Core.UI.LegacyText;

namespace Core.UI.FlowField.GridNodePresenter
{
    public class CostText : BaseText
    {
        public void SetCost(int cost) => this.text.text = cost.ToString();

        public void Clear() => this.text.text = "";

    }


}