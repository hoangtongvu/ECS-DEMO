using Core.GameResource;
using Unity.Collections;
using Unity.Entities;

namespace Components.GameResource
{
    public struct ResourceItemSpawnCommandList : IComponentData
    {
        public NativeList<ResourceItemSpawnCommand> Value;
    }

}
