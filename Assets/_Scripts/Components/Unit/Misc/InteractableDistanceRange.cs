using Unity.Entities;
using Unity.Mathematics;

namespace Components.Unit.Misc
{
    public struct InteractableDistanceRange : IComponentData
    {
        public half MinValue;
        public half MaxValue;
    }

}
