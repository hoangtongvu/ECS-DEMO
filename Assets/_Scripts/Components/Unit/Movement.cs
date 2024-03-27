using Unity.Entities;
using Unity.Mathematics;

namespace Components.Unit
{
    public struct MoveDirection : IComponentData
    {
        public float2 Value;
    }

    public struct MoveSpeed : IComponentData
    {
        public float Value;
    }

    public struct MoveableState : IComponentData, IEnableableComponent
    {
        public Entity Entity;
    }

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
