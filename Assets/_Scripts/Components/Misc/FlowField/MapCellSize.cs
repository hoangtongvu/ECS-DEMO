using Unity.Entities;

namespace Components.Misc.FlowField
{
    public struct MapCellSize : IComponentData
    {
        public float Value; // Cell is a square.
    }

}
