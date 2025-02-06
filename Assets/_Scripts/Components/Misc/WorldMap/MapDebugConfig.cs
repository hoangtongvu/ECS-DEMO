using Unity.Entities;

namespace Components.Misc.WorldMap
{
    public struct MapDebugConfig : IComponentData
    {
        public bool ShowCost;
        public bool ShowGridLines;
    }

}
