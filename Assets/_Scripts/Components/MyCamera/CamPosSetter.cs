using Unity.Entities;
using Unity.Mathematics;

namespace Components.MyCamera
{
    public struct AddPos : IComponentData
    {
        public float3 Value;
    }
}
