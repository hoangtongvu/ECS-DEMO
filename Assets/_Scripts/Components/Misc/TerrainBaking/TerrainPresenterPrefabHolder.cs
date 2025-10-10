using Unity.Entities;
using UnityEngine;

namespace Components.Misc.TerrainBaking;

public struct TerrainPresenterPrefabHolder : IComponentData
{
    public UnityObjectRef<GameObject> Value;
}