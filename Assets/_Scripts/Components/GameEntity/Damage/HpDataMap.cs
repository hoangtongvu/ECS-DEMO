using Core.GameEntity.Damage;
using Unity.Collections;
using Unity.Entities;

namespace Components.GameEntity.Damage
{
    public struct HpDataMap : IComponentData
    {
        public NativeHashMap<Entity, HpData> Value;
    }
}
