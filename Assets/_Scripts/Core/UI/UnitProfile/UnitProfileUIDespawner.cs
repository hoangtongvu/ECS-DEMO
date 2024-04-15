using UnityEngine;

namespace Core.UI.UnitProfile
{
    public class UnitProfileUIDespawner : Despawner.Despawner
    {
        [SerializeField] private UnitProfileUICtrl unitProfileCtrl;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadCtrl(ref unitProfileCtrl);
        }

        public override void DespawnObject()
        {
            UnitProfileUISpawner.Instance.Despawn(this.unitProfileCtrl);
        }

        protected override bool CanDespawn() => false;
    }
}