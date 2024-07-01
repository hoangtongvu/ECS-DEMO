using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.EntitySpawningPanel.SpawningProfileDisplay
{
    public class SpawningProfileDisplayCtrl : BaseUICtrl
    {
        [SerializeField] private Image profilePic;
        [SerializeField] private IncSpawnCountButton incSpawnCountButton;
        [SerializeField] private SpawningProgressBar progressBar;

        public Image ProfilePic => profilePic;
        public IncSpawnCountButton IncSpawnCountButton => incSpawnCountButton;
        public SpawningProgressBar ProgressBar => progressBar;


        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.profilePic);
            this.LoadComponentInCtrl(ref this.incSpawnCountButton);
            this.LoadComponentInChildren(ref this.progressBar);
        }
    }
}