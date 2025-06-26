using Unity.Entities;

namespace Components.Unit.InteractableActions
{
    // NOTE: This tag life cycle is in FixedStepSimulationSystemGroup
    public struct NewlyActionTriggeredTag : IComponentData, IEnableableComponent
    {
    }

}
