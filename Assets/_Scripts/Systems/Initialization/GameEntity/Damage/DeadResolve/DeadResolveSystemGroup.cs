using Systems.Initialization.GameEntity.Damage.PendingDeadHandle;
using Unity.Entities;

namespace Systems.Initialization.GameEntity.Damage.DeadResolve
{
    [UpdateInGroup(typeof(LifeDeadResolutionSystemGroup))]
    [UpdateAfter(typeof(PendingDeadHandleSystemGroup))]
    public partial class DeadResolveSystemGroup : ComponentSystemGroup
    {
    }
}