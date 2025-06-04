using Core.Misc;
using Core.UI.Identification;
using Core.UI.MyCanvas;

namespace Core.UI
{
    public abstract class BaseUICtrl : SaiMonoBehaviour
    {
        public UIID RuntimeUIID;
        public CanvasType CanvasType;
        public CanvasAnchorPreset CanvasAnchorPreset;

        public abstract UIType GetUIType();

    }

}