using Core.GameEntity;
using Unity.Collections;
using Unity.Entities;

namespace Components.GameEntity
{
    public struct GameEntitySizeMap : IComponentData
    {
        public NativeHashMap<Entity, GameEntitySize> Value;
    }

}
