using Core.GameEntity;
using Unity.Entities;

namespace Components.GameEntity.Misc
{
    public struct GameEntitySizeHolder : IComponentData, ICleanupComponentData
    {
        public GameEntitySize Value;
    }
}
