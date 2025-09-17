using EncosyTower.TypeWraps;
using Unity.Entities;

namespace Components.GameEntity.Damage;

[WrapType(typeof(int))]
public partial struct CurrentHp : IComponentData
{
}