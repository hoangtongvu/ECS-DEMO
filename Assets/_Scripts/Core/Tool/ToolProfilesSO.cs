using AYellowpaper.SerializedCollections;
using Core.GameEntity;
using Core.GameResource;
using UnityEngine;

namespace Core.Tool
{
    [System.Serializable]
    public class ToolProfileElement : GameEntityProfileElement
    {
        [Header("Tool ProfileElement")]
        public ToolType ToolType;

        public float SpawnDurationSeconds;

        [SerializedDictionary("ResourceType ", "Quantity")]
        public SerializedDictionary<ResourceType, uint> BaseSpawningCosts;
    }

    [CreateAssetMenu(fileName = "ToolProfilesSO", menuName = "SO/GameEntity/ToolProfilesSO")]
    public class ToolProfilesSO : GameEntityProfilesSO<ToolType, ToolProfileElement>
    {
        public static readonly string DefaultAssetPath = "Misc/ToolProfilesSO";
    }

}