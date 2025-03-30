using Core.Misc;
using Unity.Entities;

namespace Components.GameEntity.EntitySpawning
{
    public struct SpawnDuration
    {
        public float SpawnDurationSeconds;
        public float DurationCounterSeconds;
    }

    public struct EntitySpawningProfileElement : IBufferElementData
    {
        public Entity PrefabToSpawn;
        public UnityObjectRef<UnityEngine.Sprite> UnitSprite;
        public bool CanSpawnState;

        public bool CanIncSpawnCount;
        public ChangedFlagValue<int> SpawnCount;

        public SpawnDuration SpawnDuration;

    }

}
