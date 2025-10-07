using Unity.Entities;

namespace Components.GameResource.ItemPicking.Pickee;

public struct PickerEntity : IComponentData
{
    public Entity Value;
}