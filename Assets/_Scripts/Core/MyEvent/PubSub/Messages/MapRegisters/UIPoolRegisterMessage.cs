using ZBase.Foundation.PubSub;
using UnityEngine;
using Unity.Entities;
using Core.UI.Identification;
using Core.UI;
using Core.UI.MyCanvas;

namespace Core.MyEvent.PubSub.Messages
{
    public struct UIPoolRegisterMessage : IMessage
    {
        public UIType Type;
        public UnityObjectRef<GameObject> Prefab;
        public UnityObjectRef<ObjPool<BaseUICtrl>> UIPool;

        public CanvasType CanvasType;
        public CanvasAnchorPreset CanvasAnchorPreset;
    }

}