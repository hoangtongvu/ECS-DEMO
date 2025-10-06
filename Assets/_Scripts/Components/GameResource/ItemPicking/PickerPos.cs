using Unity.Entities;
using Unity.Mathematics;

namespace Components.GameResource.ItemPicking;

public struct PickerPos : IComponentData
{
    public float3 Value;
}