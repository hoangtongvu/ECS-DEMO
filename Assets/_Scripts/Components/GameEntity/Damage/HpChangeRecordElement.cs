using Unity.Entities;
using UnitGenerator;

namespace Components.GameEntity.Damage;

/// <summary>
/// Positive number -> healing, negative number -> taking damage.
/// </summary>
[UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.ValueArithmeticOperator)]
public partial struct HpChangeRecordElement : IBufferElementData
{
}
