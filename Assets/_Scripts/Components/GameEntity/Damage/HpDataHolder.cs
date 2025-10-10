using Core.GameEntity.Damage;
using System;
using TypeWrap;
using Unity.Entities;

namespace Components.GameEntity.Damage;

[WrapType(typeof(HpData))]
public partial struct HpDataHolder : ISharedComponentData, IEquatable<HpDataHolder>
{
}