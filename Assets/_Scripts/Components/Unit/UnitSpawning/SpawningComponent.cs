using Core.UI.Identification;
using Core.Unit;
using Unity.Entities;

namespace Components.Unit.UnitSpawning
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

    public struct UnitSpawningProfileElement : IBufferElementData
    {
        public Entity PrefabToSpawn;
        public UnityObjectRef<UnityEngine.Sprite> UnitSprite;
        public bool CanSpawnState;
        public int SpawnCount;
        public SpawnDuration SpawnDuration;
        public UIID? UIID;

        public UnitType UnitType;
        public ushort LocalIndex;
    }

}
