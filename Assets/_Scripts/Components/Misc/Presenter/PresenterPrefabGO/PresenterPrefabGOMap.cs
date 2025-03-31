using Core.Misc.Presenter;
using Core.Misc.Presenter.PresenterPrefabGO;
using Unity.Collections;
using Unity.Entities;

namespace Components.Misc.Presenter.PresenterPrefabGO
{
    public struct PresenterPrefabGOMap : IComponentData
    {
        public NativeHashMap<PresenterPrefabGOKey, UnityObjectRef<BasePresenter>> Value;
    }

}
