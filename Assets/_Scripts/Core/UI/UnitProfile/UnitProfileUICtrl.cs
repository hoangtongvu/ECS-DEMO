using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.UnitProfile
{
    public class UnitProfileUICtrl : SaiMonoBehaviour
    {
        [SerializeField] private Image profilePic;
        [SerializeField] private UnitProfileUIButton profileUIButton;
        [SerializeField] private UnitProfileUIDespawner despawner;

        public Image ProfilePic => profilePic;
        public UnitProfileUIButton UnitProfileUIButton => profileUIButton;
        public UnitProfileUIDespawner Despawner => despawner;


        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.profilePic);
            this.LoadComponentInCtrl(ref this.profileUIButton);
            this.LoadComponentInChildren(ref this.despawner);
        }
    }
}