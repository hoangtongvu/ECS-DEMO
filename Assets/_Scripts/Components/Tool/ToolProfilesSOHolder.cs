using Core.Tool;
using Unity.Entities;

namespace Components.Tool
{
    public struct ToolProfilesSOHolder : IComponentData
    {
        public UnityObjectRef<ToolProfilesSO> Value;
    }

}
