using Systems.Simulation.GameEntity.Interaction.Common;
using Unity.Entities;

namespace Systems.Simulation.GameEntity.Interaction.InteractionPhases
{
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    [UpdateAfter(typeof(CanUpdateHandleSystemGroup))]
    [UpdateAfter(typeof(CanCancelHandleSystemGroup))]
    public partial class PhaseTagsHandleSystemGroup : ComponentSystemGroup
    {
    }
}
