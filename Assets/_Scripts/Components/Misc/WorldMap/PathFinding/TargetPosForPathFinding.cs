using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc.WorldMap.PathFinding;

public struct TargetPosForPathFinding : IComponentData
{
    public float3 Value;
}