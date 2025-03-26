using Core.Misc.Presenter;
using Unity.Entities;

namespace Components.Misc.Presenter
{
    public struct PresentersTransformAccessArrayGOHolder : IComponentData
    {
        public UnityObjectRef<PresentersTransformAccessArrayGO> Value;
    }

}
