using Core.MyEvent.PubSub.Messages;
using ZBase.Foundation.PubSub;

namespace Core.UI.UnitProfile
{
    public class UnitProfileUIButton : BaseButton
    {
        private int buttonID;
        private MessagePublisher messagePublisher;


        protected override void OnClick()
        {
            this.messagePublisher
                .Publish(new ButtonMessage(this.buttonID));
        }

        public void SetButtonIDAndPublisher(int buttonID, MessagePublisher messagePublisher)
        {
            this.buttonID = buttonID;
            this.messagePublisher = messagePublisher;
        }

    }
}