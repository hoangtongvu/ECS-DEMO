using ZBase.Foundation.PubSub;

namespace Core.MyEvent.PubSub.Messengers
{
    public static class MapRegisterMessenger
    {
        private static readonly Messenger messenger = new();

        public static Messenger Instance => messenger;
        public static AnonPublisher AnonPublisher => messenger.AnonPublisher;
        public static AnonSubscriber AnonSubscriber => messenger.AnonSubscriber;
        public static MessagePublisher MessagePublisher => messenger.MessagePublisher;
        public static MessageSubscriber MessageSubscriber => messenger.MessageSubscriber;


    }
}