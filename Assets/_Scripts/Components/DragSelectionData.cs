using Unity.Entities;
using Unity.Mathematics;

namespace Components
{

    public struct DragSelectionData : IComponentData
    {
        public float3 StartWorldPos;
        public float3 CurrentWorldPos;
        public bool IsDragging;
        public float DistanceToConsiderDrag;
    }

}