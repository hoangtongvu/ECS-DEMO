using UnityEngine;

namespace Core.UI.FlowField.GridNodePresenter
{
    public class GridNodePresenterCtrl : BaseUICtrl
    {
        [SerializeField] private CostText costText;
        [SerializeField] private BestCostText bestCostText;
        [SerializeField] private DirectionImage directionImage;
        [SerializeField] private BackgroundImage backgroundImage;

        public CostText CostText => costText;
        public BestCostText BestCostText => bestCostText;
        public DirectionImage DirectionImage => directionImage;
        public BackgroundImage BackgroundImage => backgroundImage;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.costText);
            this.LoadComponentInChildren(ref this.bestCostText);
            this.LoadComponentInChildren(ref this.directionImage);
            this.LoadComponentInChildren(ref this.backgroundImage);

        }

    }


}