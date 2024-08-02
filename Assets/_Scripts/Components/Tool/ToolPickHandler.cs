using Unity.Entities;

namespace Components.Tool
{
    public struct CanBePicked : IComponentData
    {
        public bool Value;
    }

    public struct ToolPickerEntity : IComponentData
    {
        public Entity Value;
    }


}
