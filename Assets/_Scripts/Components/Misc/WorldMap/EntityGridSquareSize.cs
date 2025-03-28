using Unity.Entities;

namespace Components.Misc.WorldMap
{
    public struct EntityGridSquareSize : IComponentData
    {
        public int Value;

        public struct ToRegisterEntity : IComponentData
        {
            public Entity Value;
        }

    }

}
