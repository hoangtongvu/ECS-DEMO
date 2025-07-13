using Unity.Entities;

namespace Systems.Initialization.Misc.Presenter
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    public partial class NeedSpawnPresenterTagProcessSystemGroup : ComponentSystemGroup
    {
    }

}