using Unity.Entities;

namespace Systems.Initialization.Misc.Presenter
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class NeedSpawnPresenterTagProcessSystemGroup : ComponentSystemGroup
    {
    }

}