using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using UnityEngine;
using ZBase.Foundation.PubSub;
using UnityEngine.Rendering;

namespace Core.Misc
{
    public class GlobalVolumeRegister : MonoBehaviour
    {
        [SerializeField] private Volume Target;

        private void Awake()
        {
            if (!this.Target) this.Target = GetComponent<Volume>();

            MapRegisterMessenger.MessagePublisher.Publish(new RegisterMessage<Volume>
            {
                TargetRef = Target,
            });
        }

    }

}