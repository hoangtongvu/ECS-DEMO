using Unity.Entities;
using Unity.Mathematics;

namespace Components.GameResource.ItemPicking.Pickee;

public struct PickerPos : IComponentData
{
    public float3 Value;
}