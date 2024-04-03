using Core;
using Unity.Entities;
using Unity.Physics;

namespace Components
{

    public struct SelectionHitData : IComponentData
    {
        public SelectionType SelectionType;
        public RaycastHit RaycastHit;
        public bool NewlyAdded;
    }
}
