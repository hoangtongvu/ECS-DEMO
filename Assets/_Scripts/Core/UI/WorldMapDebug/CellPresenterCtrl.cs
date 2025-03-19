using UnityEngine;

namespace Core.UI.WorldMapDebug
{
    [GenerateUIType("WorldMapCellPresenter")]
    public partial class CellPresenterCtrl : BaseUICtrl
    {
        [SerializeField] private CostText costText;
        [SerializeField] private BackgroundImage backgroundImage;

        public CostText CostText => costText;
        public BackgroundImage BackgroundImage => backgroundImage;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.costText);
            this.LoadComponentInChildren(ref this.backgroundImage);
        }

    }

}