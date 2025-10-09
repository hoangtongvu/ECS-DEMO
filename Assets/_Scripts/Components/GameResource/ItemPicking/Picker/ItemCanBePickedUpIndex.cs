using EncosyTower.TypeWraps;
using Unity.Entities;

namespace Components.GameResource.ItemPicking.Picker;

[WrapType(typeof(int))]
public partial struct ItemCanBePickedUpIndex : IBufferElementData
{
}