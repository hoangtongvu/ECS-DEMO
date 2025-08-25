using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using Core.UI.MyImage;
using Unity.Entities;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.UI.InteractableActionsPanel.ActionPanel.EntitySpawningProfileActionPanel
{
    [GenerateUIType("ActionPanel_EntitySpawningProfile")]
    public partial class EntitySpawningProfileActionPanelCtrl : ActionPanelCtrl
    {
        [SerializeField] private BaseImage profilePic;
        [SerializeField] private SpawnCountText spawnCountText;
        [SerializeField] private SpawningProgressBar progressBar;

        [field: SerializeField] public int SpawningProfileElementIndex {  get; private set; }

        public BaseImage ProfilePic => profilePic;
        public SpawnCountText SpawnCountText => spawnCountText;
        public SpawningProgressBar ProgressBar => progressBar;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.profilePic);
            this.LoadComponentInChildren(ref this.spawnCountText);
            this.LoadComponentInChildren(ref this.progressBar);
        }

        public void Initialize(
            in Entity baseEntity
            , sbyte priority
            , ActionsContainerUICtrl actionsContainerUICtrl
            , int spawningProfileElementIndex)
        {
            base.Initialize(baseEntity, priority, actionsContainerUICtrl);
            this.SpawningProfileElementIndex = spawningProfileElementIndex;
        }

        public override void Activate()
        {
            GameplayMessenger.MessagePublisher
                .Publish(new SpawnUnitMessage(this.BaseEntity, this.SpawningProfileElementIndex));
        }

        public override void OnRent()
        {
        }

        public override void OnReturn()
        {
        }

    }

}