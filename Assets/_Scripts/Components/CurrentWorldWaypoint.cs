using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct CurrentWorldWaypoint : IComponentData
    {
        public float3 Value;
    }

}
