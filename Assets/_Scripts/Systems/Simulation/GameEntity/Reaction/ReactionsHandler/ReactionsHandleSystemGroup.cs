using Systems.Simulation.GameEntity.Reaction.CanUpdateConditionsHandler;
using Unity.Entities;

namespace Systems.Simulation.GameEntity.Reaction.ReactionsHandler
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(CanUpdateConditionsHandleSystemGroup))]
    public partial class ReactionsHandleSystemGroup : ComponentSystemGroup
    {
    }
}
