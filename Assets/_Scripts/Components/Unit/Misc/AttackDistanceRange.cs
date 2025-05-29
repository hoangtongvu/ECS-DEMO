using Unity.Entities;
using Unity.Mathematics;

namespace Components.Unit.Misc
{
    public struct AttackDistanceRange : IComponentData
    {
        public half MinValue;
        public half MaxValue;
    }

}
