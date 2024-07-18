using Core.UI.Identification;
using Unity.Entities;

namespace Components.MyEntity.EntitySpawning
{

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

        public bool CanIncSpawnCount;
        public ChangedFlagValue<int> SpawnCount;

        public SpawnDuration SpawnDuration;
        public UIID? UIID;

    }

}
