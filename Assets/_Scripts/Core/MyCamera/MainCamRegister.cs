using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.MyCamera
{

    public class MainCamRegister : MonoBehaviour
    {
        [SerializeField] private Camera Target;


        private void Awake()
        {
            if (this.Target == false) this.Target = this.GetComponent<Camera>();

            MapRegisterMessenger.MessagePublisher.Publish(new RegisterMessage<Camera>
            {
                TargetRef = this.Target,
            });
        }


    }
}