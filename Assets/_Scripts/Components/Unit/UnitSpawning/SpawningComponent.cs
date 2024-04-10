using Unity.Entities;

namespace Components.Unit.UnitSpawning
{

    public struct SpawnRadius : IComponentData
    {
        public float Value;
    }

    public struct SpawnDuration : IComponentData
    {
        public float DurationPerSpawn;
        public float DurationCounterSecond;
    }

    public struct UnitSpawningProfileElement : IBufferElementData
    {
        public Entity PrefabToSpawn;
        public UnityObjectRef<UnityEngine.Sprite> UnitSprite;
        public bool CanSpawnState;
        public int SpawnCount;
        public SpawnDuration SpawnDuration;

    }

}
