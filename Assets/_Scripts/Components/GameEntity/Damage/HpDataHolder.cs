using Core.GameEntity.Damage;
using System;
using Unity.Entities;
using UnitGenerator;

namespace Components.GameEntity.Damage;

[UnitOf(typeof(HpData))]
public partial struct HpDataHolder : ISharedComponentData, IEquatable<HpDataHolder>
{
}
