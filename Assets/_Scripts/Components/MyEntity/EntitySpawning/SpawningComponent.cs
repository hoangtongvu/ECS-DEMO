using Core.UI.Identification;
using Unity.Entities;

namespace Components.MyEntity.EntitySpawning
{

    public struct SpawnRadius : IComponentData
    {
        public float Value;
    }

    public struct SpawnDuration
    {
        public float DurationPerSpawn;
        public float DurationCounterSecond;
    }

    public struct EntitySpawningProfileElement : IBufferElementData
    {
        public Entity PrefabToSpawn;
        public UnityObjectRef<UnityEngine.Sprite> UnitSprite;
        public bool CanSpawnState;
        public int SpawnCount;
        public SpawnDuration SpawnDuration;
        public UIID? UIID;

    }

}
