using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Components.MyEntity.EntitySpawning.GlobalCostMap.Containers
{
    public struct EntitySpawningSpritesContainer : IComponentData
    {
        public NativeList<UnityObjectRef<Sprite>> Value;
    }

}
