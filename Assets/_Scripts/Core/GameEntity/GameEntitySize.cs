
namespace Core.GameEntity
{
    [System.Serializable]
    public struct GameEntitySize
    {
        public int GridSquareSize;
        public float ObjectHeight;

        public override string ToString() =>
            $"{nameof(GridSquareSize)}: {GridSquareSize}, {nameof(ObjectHeight)}: {ObjectHeight}";

    }

}
