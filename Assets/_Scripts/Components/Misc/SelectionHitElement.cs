using Core.Misc;
using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc
{
    public struct SelectionHitElement : IBufferElementData
    {
        public SelectionType SelectionType;
        public Entity HitEntity;
        public float3 HitPos;
    }
}
