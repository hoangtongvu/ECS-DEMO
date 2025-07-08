using Unity.Entities;

namespace Systems.Initialization.GameEntity.Misc.EntityCleanup
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class CleanupEntityHandleSystemGroup : ComponentSystemGroup
    {
    }
}