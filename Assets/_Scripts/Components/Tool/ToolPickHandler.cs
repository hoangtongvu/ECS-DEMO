using Unity.Entities;

namespace Components.Tool
{
    public struct CanBePickedTag : IComponentData, IEnableableComponent
    {
    }

    public struct ToolPickerEntity : IComponentData
    {
        public Entity Value;
    }


}
