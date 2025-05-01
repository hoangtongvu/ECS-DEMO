using UnityEngine;
using AYellowpaper.SerializedCollections;
using Core.Unit.MyMoveCommand;
using Core.GameEntity;
using Core.GameResource;
using Core.Unit.Reaction;

namespace Core.Unit
{
    [System.Serializable]
    public class UnitProfileElement : GameEntityProfileElement
    {
        [Header("Unit ProfileElement")]
        public UnitType UnitType;

        public float SpawnDurationSeconds;
        public UnitReactionConfigs UnitReactionConfigs = new();

        [SerializedDictionary("ResourceType ", "Quantity")]
        public SerializedDictionary<ResourceType, uint> BaseSpawningCosts;

        [SerializedDictionary("MoveCommandSource", "Priority")]
        public SerializedDictionary<MoveCommandSource, byte> MoveCommandSourcePriorities;

    }

    [CreateAssetMenu(fileName = "UnitProfilesSO", menuName = "SO/GameEntity/UnitProfilesSO")]
    public class UnitProfilesSO : GameEntityProfilesSO<UnitProfileId, UnitProfileElement>
    {
        public UnitGlobalConfigs UnitGlobalConfigs = new();
        public static readonly string DefaultAssetPath = "Misc/UnitProfilesSO";

#if (UNITY_EDITOR)
        [ContextMenu("Reset Global Configs")]
        private void ResetGlobalConfigs()
        {
            this.UnitGlobalConfigs = new();
        }

#endif

    }

}