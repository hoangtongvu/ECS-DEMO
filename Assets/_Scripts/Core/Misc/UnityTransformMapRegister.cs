using Core.CustomIdentification;
using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.Misc
{
    public class UnityTransformMapRegister : MonoBehaviour
    {
        [SerializeField] private UniqueId Id;
        [SerializeField] private Transform Target;

        private void Awake()
        {
            if (this.Target == false) this.Target = transform;

            MapRegisterMessenger.MessagePublisher.Publish(new RegisterMessage<UniqueId, Transform>
            {
                ID = Id,
                TargetRef = Target,
            });
        }

    }

}