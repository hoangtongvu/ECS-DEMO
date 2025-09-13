using Systems.Initialization.GameEntity.Damage.HpChangesHandle;
using Unity.Entities;

namespace Systems.Initialization.GameEntity.Damage.PendingDeadHandle
{
    [UpdateInGroup(typeof(LifeDeadResolutionSystemGroup))]
    [UpdateAfter(typeof(HpChangesHandleSystemGroup))]
    public partial class PendingDeadHandleSystemGroup : ComponentSystemGroup
    {
    }
}