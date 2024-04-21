using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.UnitProfile
{
    public class UnitProfileUICtrl : BaseUICtrl
    {
        [SerializeField] private Image profilePic;
        [SerializeField] private UnitProfileUIButton profileUIButton;
        [SerializeField] private UnitProfileUIDespawner despawner;
        [SerializeField] private UnitProfileUIProgressBar progressBar;

        public Image ProfilePic => profilePic;
        public UnitProfileUIButton UnitProfileUIButton => profileUIButton;
        public UnitProfileUIDespawner Despawner => despawner;
        public UnitProfileUIProgressBar ProgressBar => progressBar;


        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.profilePic);
            this.LoadComponentInCtrl(ref this.profileUIButton);
            this.LoadComponentInChildren(ref this.despawner);
            this.LoadComponentInChildren(ref this.progressBar);
        }
    }
}