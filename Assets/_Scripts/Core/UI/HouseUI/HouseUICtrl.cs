using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.UI.HouseUI
{
    public class HouseUICtrl : SaiMonoBehaviour
    {
        [SerializeField] private HouseUIDespawner despawner;
        [SerializeField] private UnitProfileHolder unitProfileHolder;
        private readonly Messenger messenger = new();

        public HouseUIDespawner Despawner => despawner;
        public UnitProfileHolder UnitProfileHolder => unitProfileHolder;
        public Messenger Messenger => messenger;


        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.despawner);
            this.LoadComponentInChildren(ref this.unitProfileHolder);
        }
    }
}