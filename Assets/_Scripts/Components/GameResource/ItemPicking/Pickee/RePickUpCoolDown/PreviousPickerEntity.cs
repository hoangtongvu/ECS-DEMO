using Unity.Entities;

namespace Components.GameResource.ItemPicking.Pickee.RePickUpCoolDown;

public struct PreviousPickerEntity : IComponentData
{
    public Entity Value;
}