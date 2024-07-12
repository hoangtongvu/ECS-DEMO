using Unity.Entities;

namespace Components.Tool
{
    public struct ToolCallerRadius : IComponentData
    {
        public float Value;
    }
    
    public struct ToolPickRadius : IComponentData
    {
        public float Value;
    }
}
