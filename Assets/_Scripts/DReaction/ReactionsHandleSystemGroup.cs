using Unity.Entities;

namespace DReaction
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(CanUpdateConditionsHandleSystemGroup))]
    public partial class ReactionsHandleSystemGroup : ComponentSystemGroup
    {
    }
}
