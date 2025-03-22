using Unity.Entities;

namespace Systems.Initialization.Misc.WorldMap
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(WorldMapChangedTagClearSystem))]
    public partial class MapGenerateSystemGroup : ComponentSystemGroup
    {
    }

}