using TypeWrap;
using Unity.Entities;

namespace Components.Unit.UnitFeeding;

[WrapType(typeof(float))]
public partial struct StarvingDmgDealTimerSeconds : IComponentData
{
}