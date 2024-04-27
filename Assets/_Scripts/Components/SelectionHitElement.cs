using Core;
using Unity.Entities;
using Unity.Physics;

namespace Components
{

    public struct SelectionHitElement : IBufferElementData
    {
        public SelectionType SelectionType;
        public RaycastHit RaycastHit;
    }
}
