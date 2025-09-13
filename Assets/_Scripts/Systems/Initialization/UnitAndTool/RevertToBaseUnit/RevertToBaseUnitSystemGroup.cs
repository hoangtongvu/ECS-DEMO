using Systems.Initialization.GameEntity.Damage.PendingDeadHandle;
using Unity.Entities;

namespace Systems.Initialization.UnitAndTool.RevertToBaseUnit
{
    [UpdateInGroup(typeof(PendingDeadHandleSystemGroup))]
    public partial class RevertToBaseUnitSystemGroup : ComponentSystemGroup
    {
    }
}
