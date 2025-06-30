using Unity.Entities;

namespace Systems.Simulation.GameEntity.InteractableActions
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(ActionsTriggeredSystemGroup))]
    public partial class ActionUIsHandleSystemGroup : ComponentSystemGroup
    {
    }

}