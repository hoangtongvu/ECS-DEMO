using Unity.Entities;

namespace DInteraction.Common
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(CanUpdateHandleSystemGroup))]
    [UpdateAfter(typeof(CanCancelHandleSystemGroup))]
    public partial class PhaseTagsHandleSystemGroup : ComponentSystemGroup
    {
    }
}
