using Core.Misc;
using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core.UI.MyCanvas
{
    public class CanvasesCtrlRegister : SaiMonoBehaviour
    {
        [SerializeField] private CanvasesCtrl Target;

        protected override void Awake()
        {
            base.Awake();

            if (this.Target == false) this.Target = this.GetComponent<CanvasesCtrl>();

            MapRegisterMessenger.MessagePublisher.Publish(new RegisterMessage<CanvasesCtrl>
            {
                TargetRef = this.Target,
            });

        }

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.Target = this.GetComponent<CanvasesCtrl>();
        }

    }

}