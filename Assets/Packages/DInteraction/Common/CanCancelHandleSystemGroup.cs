using Unity.Entities;

namespace DInteraction.Common
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class CanCancelHandleSystemGroup : ComponentSystemGroup
    {
    }
}
