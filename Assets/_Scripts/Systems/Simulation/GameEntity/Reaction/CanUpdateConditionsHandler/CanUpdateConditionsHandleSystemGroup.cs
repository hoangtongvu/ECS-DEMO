using Unity.Entities;

namespace Systems.Simulation.GameEntity.Reaction.CanUpdateConditionsHandler
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class CanUpdateConditionsHandleSystemGroup : ComponentSystemGroup
    {
    }
}
