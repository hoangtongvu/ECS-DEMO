using Unity.Entities;

namespace Components.Tool
{
    public struct CanBePicked : IComponentData
    {
        public bool Value;
    }

    public struct PickedBy : IComponentData
    {
        public Entity Value;
    }


}
