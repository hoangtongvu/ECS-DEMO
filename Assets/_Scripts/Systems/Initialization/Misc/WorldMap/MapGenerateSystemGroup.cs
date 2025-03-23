using Unity.Entities;

namespace Systems.Initialization.Misc.WorldMap
{
    [UpdateInGroup(typeof(MapChangedSystemGroup))]
    public partial class MapGenerateSystemGroup : ComponentSystemGroup
    {
    }

}