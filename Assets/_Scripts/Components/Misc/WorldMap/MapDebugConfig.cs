using Unity.Entities;

namespace Components.Misc.WorldMap
{
    public struct MapDebugConfig : IComponentData
    {
        public bool ShowCellCosts;
        public bool ShowCellGridLines;
        public bool ShowChunks;
    }

}
