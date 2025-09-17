using Core.Unit.UnitFeeding;
using EncosyTower.TypeWraps;
using Unity.Entities;

namespace Components.Unit.UnitFeeding;

[WrapType(typeof(HungerThreshold))]
public partial struct CurrentHungerThreshold : IComponentData
{
}