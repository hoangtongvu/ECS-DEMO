using Core.MyEvent.PubSub.Messages;
using Core.MyEvent.PubSub.Messengers;
using Core.UI;
using Core.UI.Identification;
using Core.UI.MyCanvas;
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
                CanvasType canvasType = uiCtrl.CanvasType;
                CanvasAnchorPreset canvasAnchorPreset = uiCtrl.CanvasAnchorPreset;

                this.InstantiateObjPool(
                    type
                    , canvasType
                    , canvasAnchorPreset
                    , out var objPool);

                MapRegisterMessenger.MessagePublisher.Publish(new UIPoolRegisterMessage
                {
                    Type = type,
                    Prefab = uiCtrl.gameObject,
                    UIPool = objPool,

                    CanvasType = canvasType,
                    CanvasAnchorPreset = canvasAnchorPreset,
                });
            }
        }


        private void LoadAllUIPrefabs() => this.uiCtrls = Resources.LoadAll<BaseUICtrl>("UI");

        private void InstantiateObjPool(
            UIType type
            , CanvasType canvasType
            , CanvasAnchorPreset canvasAnchorPreset
            , out BaseUIPool objPool)
        {
            GameObject newGameObject = new($"{type} Pool");
            newGameObject.AddComponent<RectTransform>();

            newGameObject.transform.SetParent(this.GetParentTransform(canvasType, canvasAnchorPreset));
            newGameObject.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

            objPool = newGameObject.AddComponent<BaseUIPool>();
        }

        private Transform GetParentTransform(CanvasType canvasType, CanvasAnchorPreset canvasAnchorPreset)
        {
            switch (canvasType)
            {
                case CanvasType.WorldSpace:
                    return CanvasesCtrl.Instance.WorldSpaceCanvasTransform;

                case CanvasType.Overlay:
                    int index = (int) canvasAnchorPreset;
                    return CanvasesCtrl.Instance.OverlayCanvasManager.GetAnchorPresetTransform(index);

                default:
                    return this.transform;
            }
        }

        // Load UI Prefabs from Resources.
        // Instantiate objPool for each UIPrefab.
        // Set objPool's parent to ObjPoolsHolder.
        // With each prefab, GetComponent BaseUICtrl and get UIType from that.
        // Publish Message.
    }
}