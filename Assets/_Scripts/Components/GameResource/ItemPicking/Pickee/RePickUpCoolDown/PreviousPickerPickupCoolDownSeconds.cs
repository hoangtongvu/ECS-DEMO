using TypeWrap;
using Unity.Entities;

namespace Components.GameResource.ItemPicking.Pickee.RePickUpCoolDown;

[WrapType(typeof(float))]
public partial struct PreviousPickerPickupCoolDownSeconds : IComponentData
{
}