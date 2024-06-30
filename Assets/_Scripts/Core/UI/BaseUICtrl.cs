using Core.UI.Identification;
using Core.UI.MyCanvas;

namespace Core.UI
{
    public abstract class BaseUICtrl : SaiMonoBehaviour
    {
        public UIID UIID = new();
        public CanvasType CanvasType;
        public CanvasAnchorPreset CanvasAnchorPreset;

    }
}