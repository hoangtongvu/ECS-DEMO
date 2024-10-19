using Core.Misc.Presenter;
using Unity.Entities;

namespace Components.Misc.Presenter
{
    public struct PresenterHolder : IComponentData
    {
        public UnityObjectRef<BasePresenter> Value;
    }

}
