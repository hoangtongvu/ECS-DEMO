using Core.UI.MyCanvas;
using Unity.Entities;

namespace Components.UI.MyCanvas
{
    public struct CanvasesCtrlHolder : IComponentData
    {
        public UnityObjectRef<CanvasesCtrl> Value;
    }

}
