using Unity.Entities;
using UnitGenerator;

namespace Components.Unit.UnitFeeding;

[UnitOf(typeof(float), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.ValueArithmeticOperator)]
public partial struct StarvingDmgDealTimerSeconds : IComponentData
{
}