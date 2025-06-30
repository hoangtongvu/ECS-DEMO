using Unity.Entities;

namespace Components.GameEntity.InteractableActions
{
    // NOTE: This tag life cycle is in FixedStepSimulationSystemGroup
    public struct NewlyActionTriggeredTag : IComponentData, IEnableableComponent
    {
    }

}
