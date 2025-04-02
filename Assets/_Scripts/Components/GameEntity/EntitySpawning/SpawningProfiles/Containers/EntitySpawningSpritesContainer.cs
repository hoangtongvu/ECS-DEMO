using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Components.GameEntity.EntitySpawning.SpawningProfiles.Containers
{
    public struct EntitySpawningSpritesContainer : IComponentData
    {
        public NativeList<UnityObjectRef<Sprite>> Value;
    }

}
