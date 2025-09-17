using EncosyTower.TypeWraps;
using Unity.Entities;

namespace Components.Unit.UnitFeeding;

[WrapType(typeof(float))]
public partial struct FeedingTimerSeconds : IComponentData
{
}