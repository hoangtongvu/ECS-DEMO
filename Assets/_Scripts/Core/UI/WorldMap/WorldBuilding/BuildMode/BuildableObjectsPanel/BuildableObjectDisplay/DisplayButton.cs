using Core.MyEvent.PubSub.Messages.WorldBuilding;
using Core.MyEvent.PubSub.Messengers;
using Core.UI.WorldMap.BuildableObjects.BuildableObjectsPanel.BuildableObjectDisplay;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.UI.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel.BuildableObjectDisplay
{
    public class DisplayButton : BaseButton
    {
        [SerializeField] private BuildableObjectDisplayCtrl ctrl;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadCtrl(ref this.ctrl);
        }

        protected override void OnClick()
        {
            GameplayMessenger.MessagePublisher
                .Publish(new ChooseBuildableObjectMessage
                {
                    choiceIndex = ctrl.IndexInDisplaysHolder,
                });

        }

    }

}
