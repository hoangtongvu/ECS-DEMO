using Core.GameResource;
using Unity.Collections;
using Unity.Entities;

namespace Components.GameResource
{
    public struct ResourceItemPresenterEntityPrefabMap : IComponentData
    {
        public NativeHashMap<ResourceProfileId, Entity> Value;
    }

}
