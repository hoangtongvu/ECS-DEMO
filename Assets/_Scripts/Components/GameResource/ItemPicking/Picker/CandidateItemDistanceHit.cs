using EncosyTower.TypeWraps;
using Unity.Entities;
using Unity.Physics;

namespace Components.GameResource.ItemPicking.Picker;

[WrapType(typeof(DistanceHit))]
public partial struct CandidateItemDistanceHit : IBufferElementData
{
}