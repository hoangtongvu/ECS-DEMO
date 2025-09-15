using Unity.Entities;
using UnitGenerator;

namespace Components.GameEntity.Damage;

[UnitOf(typeof(int), UnitGenerator.UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.ValueArithmeticOperator)]
public partial struct FrameHpChange : IComponentData
{
}