using Unity.Entities;

namespace Systems.Initialization.GameEntity.Damage
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class HpChangesHandleSystemGroup : ComponentSystemGroup
    {
    }

}