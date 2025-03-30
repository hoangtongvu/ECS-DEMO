using UnityEngine;
using AYellowpaper.SerializedCollections;
using Core.Unit.MyMoveCommand;
using Core.GameEntity;
using Core.GameResource;

namespace Core.Unit
{
    [System.Serializable]
    public class UnitProfileElement : GameEntityProfileElement
    {
        [Header("Unit ProfileElement")]
        public UnitType UnitType;

        public float SpawnDurationSeconds;

        [SerializedDictionary("ResourceType ", "Quantity")]
        public SerializedDictionary<ResourceType, uint> BaseSpawningCosts;

        [SerializedDictionary("MoveCommandSource", "Priority")]
        public SerializedDictionary<MoveCommandSource, byte> MoveCommandSourcePriorities;

    }

    [CreateAssetMenu(fileName = "UnitProfilesSO", menuName = "SO/GameEntity/UnitProfilesSO")]
    public class UnitProfilesSO : GameEntityProfilesSO<UnitId, UnitProfileElement>
    {
        public static readonly string DefaultAssetPath = "Misc/UnitProfilesSO";
    }

}