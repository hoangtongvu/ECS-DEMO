using Core.Misc.Presenter;
using Unity.Collections;
using Unity.Entities;

namespace Components.Misc.Presenter
{
    public struct PresenterPrefabMap : IComponentData
    {
        public NativeHashMap<PresenterPrefabId, UnityObjectRef<BasePresenter>> Value;
    }

}
