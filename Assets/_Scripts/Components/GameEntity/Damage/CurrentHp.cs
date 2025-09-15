using UnitGenerator;
using Unity.Entities;

namespace Components.GameEntity.Damage;

[UnitOf(typeof(int), UnitGenerateOptions.ImplicitOperator | UnitGenerateOptions.ValueArithmeticOperator | UnitGenerateOptions.MinMaxMethod_UnityMathematics)]
public partial struct CurrentHp : IComponentData
{
}
