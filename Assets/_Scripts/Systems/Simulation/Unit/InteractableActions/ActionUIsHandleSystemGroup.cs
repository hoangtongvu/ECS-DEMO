using Unity.Entities;

namespace Systems.Simulation.Unit.InteractableActions
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class ActionUIsHandleSystemGroup : ComponentSystemGroup
    {
    }

}