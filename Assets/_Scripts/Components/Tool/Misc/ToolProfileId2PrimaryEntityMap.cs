using Core.Tool;
using Unity.Collections;
using Unity.Entities;

namespace Components.Tool.Misc
{
    public struct ToolProfileId2PrimaryEntityMap : IComponentData
    {
        public NativeHashMap<ToolProfileId, Entity> Value;
    }
}
