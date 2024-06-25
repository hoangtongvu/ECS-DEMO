using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using Core.UI;
using Core.UI.Identification;
using UnityEngine;
using ZBase.Foundation.PubSub;

namespace Core
{

    public class UIPoolMapRegister : MonoBehaviour
    {

        [SerializeField] private BaseUICtrl[] uiCtrls;


        private void Awake()
        {
            this.LoadAllUIPrefabs();
            this.Register();
        }

        private void Register()
        {
            foreach (var uiCtrl in this.uiCtrls)
            {
                UIType type = uiCtrl.UIID.Type;

                this.InstantiateObjPool(type, out var objPool);

                MapRegisterMessenger.MessagePublisher.Publish(new UIPoolRegisterMessage
                {
                    Type = type,
                    Prefab = uiCtrl.gameObject,
                    UIPool = objPool,
                });
            }
        }


        private void LoadAllUIPrefabs() => this.uiCtrls = Resources.LoadAll<BaseUICtrl>("UI");

        private void InstantiateObjPool(UIType type, out BaseUIPool objPool)
        {
            GameObject newGameObject = new($"{type} Pool");
            newGameObject.transform.SetParent(this.transform);
            objPool = newGameObject.AddComponent<BaseUIPool>();
        }

        // Load UI Prefabs from Resources.
        // Instantiate objPool for each UIPrefab.
        // Set objPool's parent to ObjPoolsHolder.
        // With each prefab, GetComponent BaseUICtrl and get UIType from that.
        // Publish Message.
    }
}