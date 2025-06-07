using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Components.Misc.Presenter.PresenterPrefabGO
{
    public struct OriginalPresenterGOMap : IComponentData
    {
        public NativeHashMap<Entity, UnityObjectRef<GameObject>> Value;
    }

}
