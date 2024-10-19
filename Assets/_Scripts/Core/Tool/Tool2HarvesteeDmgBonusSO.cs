using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Core.Tool
{
    [CreateAssetMenu(fileName = "Tool2HarvesteeDmgBonusSO", menuName = "SO/Misc/Tool2HarvesteeDmgBonusSO")]
    public class Tool2HarvesteeDmgBonusSO : ScriptableObject
    {
        public static string DefaultAssetPath = "Misc/Tool2HarvesteeDmgBonusSO";

        [SerializedDictionary("ToolHarvesteePairId", "BonusValue")]
        public SerializedDictionary<ToolHarvesteePairId, float> BonusMap;
    }
}