using Core.UI.LegacyText;

namespace Core.UI.FlowField.GridNodePresenter
{
    public class BestCostText : BaseText
    {
        public void SetBestCost(int bestCost) => this.text.text = bestCost.ToString();

        public void Clear() => this.text.text = "";

    }


}