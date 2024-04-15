using Core.UI.HouseUI;
using Unity.Entities;

namespace Components
{

    public struct HouseUICtrlRef : IComponentData
    {
        public UnityObjectRef<HouseUICtrl> Value;
    }

}
