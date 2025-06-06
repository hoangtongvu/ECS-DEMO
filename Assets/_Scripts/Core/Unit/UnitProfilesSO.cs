using UnityEngine;
using AYellowpaper.SerializedCollections;
using Core.GameEntity;
using Core.GameResource;
using Core.Unit.Reaction;
using Core.Unit.Misc;

namespace Core.Unit
{
    [System.Serializable]
    public class UnitProfileElement : GameEntityProfileElement
    {
        [Header("Unit ProfileElement")]
        public UnitType UnitType;

        public float SpawnDurationSeconds;
        public UnitReactionConfigs UnitReactionConfigs = new();

        public float MaxFollowDistance = 14f;
        public float DetectionRadius = 10f;
        public AttackConfigsFloat AttackConfigs = new();

        [SerializedDictionary("ResourceType ", "Quantity")]
        public SerializedDictionary<ResourceType, uint> BaseSpawningCosts;

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