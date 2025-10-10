using Core.Unit.UnitFeeding;
using TypeWrap;
using Unity.Entities;

namespace Components.Unit.UnitFeeding;

[WrapType(typeof(HungerThreshold))]
public partial struct CurrentHungerThreshold : IComponentData
{
}