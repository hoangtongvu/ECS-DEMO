using Core.GameResource;
using Unity.Collections;
using Unity.Entities;

namespace Components.GameResource.Misc;

public struct MaxQuantityPerStackMap : IComponentData
{
    public NativeHashMap<ResourceProfileId, uint> Value;
}