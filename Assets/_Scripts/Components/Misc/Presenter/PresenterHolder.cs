using Core.Misc.Presenter;
using Unity.Entities;

namespace Components.Misc.Presenter
{
    public struct PresenterHolder : IComponentData, ICleanupComponentData
    {
        public UnityObjectRef<BasePresenter> Value;
    }

}
