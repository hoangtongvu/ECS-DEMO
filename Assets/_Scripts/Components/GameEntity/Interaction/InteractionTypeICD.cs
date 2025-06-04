using Core.GameEntity;
using Unity.Entities;

namespace Components.GameEntity.Interaction
{
    public struct InteractionTypeICD : IComponentData
    {
        public InteractionType Value;
    }
}
