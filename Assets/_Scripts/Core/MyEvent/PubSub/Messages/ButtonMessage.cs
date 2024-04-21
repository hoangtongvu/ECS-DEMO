using ZBase.Foundation.PubSub;

namespace Core.MyEvent.PubSub.Messages
{

    public readonly struct ButtonMessage : IMessage
    {
        public readonly int id;
        public ButtonMessage(int id)
        {
            this.id = id;
        }
    }

}