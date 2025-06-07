using Core.Misc.Presenter;
using Unity.Entities;

namespace Components.Misc.Presenter
{
    public struct PresenterHandSlotsHolder : IComponentData
    {
        public UnityObjectRef<HandSlotMarker> RightHand;
        public UnityObjectRef<HandSlotMarker> LeftHand;
    }

}
