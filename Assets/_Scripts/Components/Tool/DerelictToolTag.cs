using Unity.Entities;

namespace Components.Tool
{
    public struct DerelictToolTag : IComponentData, IEnableableComponent
    {
    }

    // TODO: Rename this later and move it to another file.
    public struct ToolCallRadiusSingleton : IComponentData
    {
        public float Value;
    }
    
}
