using UnityEngine;

namespace Core.UI.HouseUI
{
    public class HouseUICtrl : BaseUICtrl
    {
        [SerializeField] private HouseUIDespawner despawner;
        [SerializeField] private UnitProfileHolder unitProfileHolder;

        public HouseUIDespawner Despawner => despawner;
        public UnitProfileHolder UnitProfileHolder => unitProfileHolder;


        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.despawner);
            this.LoadComponentInChildren(ref this.unitProfileHolder);
        }
    }
}