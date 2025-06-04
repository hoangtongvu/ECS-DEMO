using Unity.Entities;
using Unity.Mathematics;

namespace Components.Unit.Misc
{
    public struct DetectDangerAndRunAwaySystemTimer : IComponentData
    {
        public half TimeCounterSeconds;
        public half TimeLimitSeconds;
    }

}
