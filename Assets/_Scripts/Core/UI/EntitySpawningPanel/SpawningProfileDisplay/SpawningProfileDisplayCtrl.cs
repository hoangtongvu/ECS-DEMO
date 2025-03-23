using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.EntitySpawningPanel.SpawningProfileDisplay
{
    [GenerateUIType("SpawningProfileDisplay")]
    public partial class SpawningProfileDisplayCtrl : BaseUICtrl
    {
        [SerializeField] private Image profilePic;
        [SerializeField] private IncSpawnCountButton incSpawnCountButton;
        [SerializeField] private SpawningProgressBar progressBar;
        [SerializeField] private SpawnCountText spawnCountText;

        public Image ProfilePic => profilePic;
        public IncSpawnCountButton IncSpawnCountButton => incSpawnCountButton;
        public SpawningProgressBar ProgressBar => progressBar;
        public SpawnCountText SpawnCountText => spawnCountText;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.profilePic);
            this.LoadComponentInCtrl(ref this.incSpawnCountButton);
            this.LoadComponentInChildren(ref this.progressBar);
            this.LoadComponentInChildren(ref this.spawnCountText);
        }

    }

}