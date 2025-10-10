using Unity.Entities;
using UnityEngine;

namespace Components.Misc.TerrainBaking;

public struct TerrainPrefabHolder : IComponentData
{
    public UnityObjectRef<Terrain> Value;
}