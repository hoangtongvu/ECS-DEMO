using Unity.Entities;

namespace Components.Unit.InteractableActions
{
    public struct NearestInteractableEntity : IComponentData
    {
        public Entity Value;
    }

}
