using Core.Animator;
using Core.CustomIdentification;
using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core
{
    public class BaseAnimatorMapRegister : MonoBehaviour
    {
        [SerializeField] private UniqueId Id;
        [SerializeField] private BaseAnimator Target;


        private void Awake()
        {

            if (this.Target == false) this.Target = this.gameObject.GetComponent<BaseAnimator>();

            MapRegisterMessenger.MessagePublisher.Publish(new RegisterMessage<UniqueId, BaseAnimator>
            {
                ID = this.Id,
                TargetRef = this.Target,
            });
        }


    }
}