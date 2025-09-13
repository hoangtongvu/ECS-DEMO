using Unity.Entities;

namespace Systems.Initialization.GameEntity.Damage.HpChangesHandle
{
    [UpdateInGroup(typeof(LifeDeadResolutionSystemGroup))]
    public partial class HpChangesHandleSystemGroup : ComponentSystemGroup
    {
    }
}