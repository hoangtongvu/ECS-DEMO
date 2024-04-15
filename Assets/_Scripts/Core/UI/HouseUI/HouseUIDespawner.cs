using UnityEngine;

namespace Core.UI.HouseUI
{
    public class HouseUIDespawner : Despawner.Despawner
    {
        [SerializeField] private HouseUICtrl houseUICtrl;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadCtrl(ref houseUICtrl);
        }

        public override void DespawnObject()
        {
            HouseUISpawner.Instance.Despawn(this.houseUICtrl);
        }

        protected override bool CanDespawn() => false;
    }
}