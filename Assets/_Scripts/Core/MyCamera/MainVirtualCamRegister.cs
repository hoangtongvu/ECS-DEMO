using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using UnityEngine;
using ZBase.Foundation.PubSub;
using Cinemachine;

namespace Core.MyCamera
{
    public class MainVirtualCamRegister : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera Target;

        private void Awake()
        {
            if (this.Target == false) this.Target = this.GetComponent<CinemachineVirtualCamera>();

            MapRegisterMessenger.MessagePublisher.Publish(new RegisterMessage<CinemachineVirtualCamera>
            {
                TargetRef = this.Target,
            });
        }

    }

}