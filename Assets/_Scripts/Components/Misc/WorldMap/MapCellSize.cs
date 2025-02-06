using Unity.Entities;

namespace Components.Misc.WorldMap
{
    public struct MapCellSize : IComponentData
    {
        public float Value; // Cell is a square.
    }

}
