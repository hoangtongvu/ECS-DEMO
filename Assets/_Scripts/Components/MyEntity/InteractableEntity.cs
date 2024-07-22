using Unity.Entities;

namespace Components.MyEntity
{
    public struct InteractableEntityTag : IComponentData
    {
    }

    public struct CanInteractEntityTag : IComponentData, IEnableableComponent
    {
    }

    public struct TargetEntity : IComponentData
    {
        public Entity Value;
    }

}
