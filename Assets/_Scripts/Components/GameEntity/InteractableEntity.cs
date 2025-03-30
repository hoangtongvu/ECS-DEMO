using Core.GameEntity;
using Unity.Entities;

namespace Components.GameEntity
{
    public struct InteractableEntityTag : IComponentData
    {
    }

    public struct CanInteractEntityTag : IComponentData, IEnableableComponent
    {
    }

    public struct InteractingEntity : IComponentData
    {
        public Entity Value;
    }

    public struct InteractionTypeICD : IComponentData
    {
        public InteractionType Value;
    }

    public struct TargetEntity : IComponentData
    {
        public Entity Value;
    }

}
