using UnityEngine;
using AYellowpaper.SerializedCollections;
using Core.Tool;
using SOConstantsGenerator;

namespace Core.Unit.DarkUnit
{
    [System.Serializable]
    public struct DarkUnitProfileElement
    {
        public float SpawnRate;
        public ToolProfileId ToolProfileId;
    }

    [GenerateConstantsFor("DarkUnitConfigConstants", "Core.Unit.DarkUnit")]
    [CreateAssetMenu(fileName = "DarkUnitConfigsSO", menuName = "SO/GameEntity/DarkUnitConfigsSO")]
    public partial class DarkUnitConfigsSO : ScriptableObject
    {
        public static readonly string DefaultAssetPath = "Misc/DarkUnitConfigsSO";

        [ConstantField] public float SpawnRadius = 40f;
        [ConstantField] public float SpawnDurationMinutes = 10;
        [ConstantField] public byte DefaultDarkUnitFactionIndex = 2;

        [SerializedDictionary("Id", "Profile")]
        public SerializedDictionary<UnitProfileId, DarkUnitProfileElement> DarkUnitProfiles;

    }

}