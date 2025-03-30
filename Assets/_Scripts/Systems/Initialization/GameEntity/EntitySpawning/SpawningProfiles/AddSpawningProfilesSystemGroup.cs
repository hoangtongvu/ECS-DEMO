using Unity.Entities;
using Unity.Scenes;

namespace Systems.Initialization.GameEntity.EntitySpawning.SpawningProfiles
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(SceneSystemGroup))]
    public partial class AddSpawningProfilesSystemGroup : ComponentSystemGroup
    {

    }

}