using Core.GameEntity.Damage;
using EncosyTower.TypeWraps;
using System;
using Unity.Entities;

namespace Components.GameEntity.Damage;

[WrapType(typeof(HpData))]
public partial struct HpDataHolder : ISharedComponentData, IEquatable<HpDataHolder>
{
}