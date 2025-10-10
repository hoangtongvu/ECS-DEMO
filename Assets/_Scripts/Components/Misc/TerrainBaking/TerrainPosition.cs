using TypeWrap;
using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc.TerrainBaking;

[WrapType(typeof(float3))]
public partial struct TerrainPosition : IComponentData
{
}