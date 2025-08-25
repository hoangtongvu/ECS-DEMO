using Core.Misc;
using Core.UI.Identification;
using Core.UI.MyCanvas;
using Core.UI.Pooling;
using DSPool;

namespace Core.UI
{
    public abstract class BaseUICtrl : SaiMonoBehaviour, IPoolElement
    {
        public UIID RuntimeUIID;
        public CanvasType CanvasType;
        public CanvasAnchorPreset CanvasAnchorPreset;

        public abstract UIType GetUIType();

        public void ReturnSelfToPool() => UICtrlPoolMap.Instance.Return(this);

        public abstract void OnRent();
        public abstract void OnReturn();

    }

}