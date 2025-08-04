using Unity.Entities;

namespace Systems.Initialization.Unit.Recruit
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class RecruitableTagsHandleSystemGroup : ComponentSystemGroup
    {
    }
}