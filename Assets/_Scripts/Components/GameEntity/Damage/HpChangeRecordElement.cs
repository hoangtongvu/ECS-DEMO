using TypeWrap;
using Unity.Entities;

namespace Components.GameEntity.Damage;

/// <summary>
/// Positive number -> healing, negative number -> taking damage.
/// </summary>
[WrapType(typeof(int))]
public partial struct HpChangeRecordElement : IBufferElementData
{
}