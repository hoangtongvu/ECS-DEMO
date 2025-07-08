using Unity.Entities;
using Unity.Mathematics;

namespace Components.GameEntity.Misc.EntityCleanup
{
    public struct PendingCleanupEntityTimer : IComponentData, ICleanupComponentData
    {
        public double TimeStamp;
        public half DurationSeconds;
    }

}
