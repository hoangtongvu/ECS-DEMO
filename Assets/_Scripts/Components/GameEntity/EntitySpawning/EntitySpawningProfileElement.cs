using Core.Misc;
using Unity.Entities;

namespace Components.GameEntity.EntitySpawning
{
    public struct EntitySpawningProfileElement : IBufferElementData
    {
        public Entity PrefabToSpawn;
        public bool CanSpawnState;

        public ChangedFlagValue<int> SpawnCount;

        public float DurationCounterSeconds;

    }

}
