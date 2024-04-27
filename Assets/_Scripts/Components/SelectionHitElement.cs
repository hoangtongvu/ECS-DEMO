using Core;
using Unity.Entities;
using Unity.Mathematics;

namespace Components
{

    public struct SelectionHitElement : IBufferElementData
    {
        public SelectionType SelectionType;
        public Entity HitEntity;
        public float3 HitPos;
    }
}
