using Unity.Entities;
using Unity.Mathematics;

namespace Components
{

    public struct TargetPosition : IComponentData
    {
        public float3 Value;
    }

    public struct DistanceToTarget : IComponentData
    {
        public float CurrentDistance;
        public float MinDistance;
    }


    
}
