using Unity.Entities;

namespace Components.GameEntity.Damage
{
    // Note: Only read this in DeadResolveSystemGroup as some entities will be destroyed after the DeadEvent is fired,
    // making DeadEvent is also destroyed because it is not ICleanupComponent
    public struct DeadEvent : IComponentData, IEnableableComponent
    {
    }
}
