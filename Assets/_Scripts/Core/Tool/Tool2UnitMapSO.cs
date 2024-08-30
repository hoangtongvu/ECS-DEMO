using AYellowpaper.SerializedCollections;
using Core.Unit;
using UnityEngine;

namespace Core.Tool
{
    [CreateAssetMenu(fileName = "Tool2UnitMap", menuName = "SO/MyEntity/Tool2UnitMap")]
    public class Tool2UnitMapSO : ScriptableObject
    {
        [SerializedDictionary("ToolType", "UnitType")]
        public SerializedDictionary<ToolType, UnitType> Map;
    }
}