using Unity.Entities;

namespace Components.GameEntity.InteractableActions
{
    public struct NearestInteractableEntity : IComponentData
    {
        public Entity Value;
    }

}
