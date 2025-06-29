using Unity.Entities;

namespace Systems.Simulation.GameEntity.InteractableActions
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class ActionsTriggeredSystemGroup : ComponentSystemGroup
    {
    }

}