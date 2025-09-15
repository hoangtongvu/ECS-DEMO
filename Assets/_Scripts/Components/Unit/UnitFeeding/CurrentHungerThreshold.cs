using Core.Unit.UnitFeeding;
using Unity.Entities;
using UnitGenerator;

namespace Components.Unit.UnitFeeding;

[UnitOf(typeof(HungerThreshold), UnitGenerateOptions.ImplicitOperator)]
public partial struct CurrentHungerThreshold : IComponentData
{
}