using Unity.Entities;

namespace Systems.Simulation.GameEntity.Interaction.Common
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public partial class CanUpdateHandleSystemGroup : ComponentSystemGroup
    {
    }
}
