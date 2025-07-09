using UnityEngine;
using AYellowpaper.SerializedCollections;
using Core.Tool;

namespace Core.Unit.DarkUnit
{
    [System.Serializable]
    public struct DarkUnitProfileElement
    {
        public float SpawnRate;
        public ToolProfileId ToolProfileId;
    }

    [CreateAssetMenu(fileName = "DarkUnitConfigsSO", menuName = "SO/GameEntity/DarkUnitConfigsSO")]
    public class DarkUnitConfigsSO : ScriptableObject
    {
        public static readonly string DefaultAssetPath = "Misc/DarkUnitConfigsSO";

        public float SpawnRadius = 40f;
        public float SpawnDurationMinutes = 10;

        [SerializedDictionary("Id", "Profile")]
        public SerializedDictionary<UnitProfileId, DarkUnitProfileElement> DarkUnitProfiles;

    }

}