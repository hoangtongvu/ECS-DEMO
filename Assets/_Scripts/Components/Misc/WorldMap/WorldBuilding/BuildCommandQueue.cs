using Core.GameEntity;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc.WorldMap.WorldBuilding
{
    public struct BuildCommand
    {
        public Entity Entity;
        public int2 TopLeftCellGridPos;
        public GameEntitySize GameEntitySize;
        public Entity SpawnerEntity;
    }

    public struct BuildCommandQueue : IComponentData
    {
        public NativeList<BuildCommand> Value;
    }

}
