using Unity.Mathematics;

namespace Core.Misc.WorldMap
{
    [System.Serializable]
    public struct Chunk
    {
        public int2 TopLeftCellPos;
        public int2 BottomRightCellPos;
    }

}
