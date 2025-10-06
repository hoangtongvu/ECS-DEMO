using Unity.Entities;

namespace Components.GameResource.ItemPicking;

public struct PickerEntity : IComponentData
{
    public Entity Value;
}