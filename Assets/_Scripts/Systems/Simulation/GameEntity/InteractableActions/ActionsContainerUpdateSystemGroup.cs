using Systems.Simulation.GameEntity.InteractableActions.ActionsContainer;
using Unity.Entities;

namespace Systems.Simulation.GameEntity.InteractableActions
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ActionsContainer_ShowSystem))]
    public partial class ActionsContainerUpdateSystemGroup : ComponentSystemGroup
    {
    }
}