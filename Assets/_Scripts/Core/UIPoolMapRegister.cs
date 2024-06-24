using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using Core.UI;
using Core.UI.Identification;
using System.Collections.Generic;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core
{

    public class UIPoolMapRegister : MonoBehaviour
    {

        [System.Serializable]
        private class RegisterValue
        {
            public UIType Type;
            public GameObject Prefab;
            public ObjPool<BaseUICtrl> UIPool;
        }


        [SerializeField] private List<RegisterValue> registerValues;


        private void Awake()
        {
            if (this.registerValues.Count == 0) return;
            this.Register();
        }

        private void Register()
        {
            foreach (var value in this.registerValues)
            {
                MapRegisterMessenger.MessagePublisher.Publish(new UIPoolRegisterMessage
                {
                    Type = value.Type,
                    Prefab = value.Prefab,
                    UIPool = value.UIPool,
                });
            }
        }


    }
}