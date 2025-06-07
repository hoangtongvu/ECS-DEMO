using Core.Misc.Presenter;
using Unity.Collections;
using Unity.Entities;

namespace Components.Misc.Presenter.PresenterPrefabGO
{
    public struct PresenterPrefabGOMap : IComponentData
    {
        public NativeHashMap<Entity, UnityObjectRef<BasePresenter>> Value;
    }

}
