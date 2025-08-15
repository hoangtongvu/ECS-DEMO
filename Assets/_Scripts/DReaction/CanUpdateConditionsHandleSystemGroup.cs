using Unity.Entities;

namespace DReaction
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class CanUpdateConditionsHandleSystemGroup : ComponentSystemGroup
    {
    }
}
