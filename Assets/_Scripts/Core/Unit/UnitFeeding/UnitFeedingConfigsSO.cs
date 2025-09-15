using UnityEngine;

namespace Core.Unit.UnitFeeding
{
    [CreateAssetMenu(fileName = "UnitFeedingConfigsSO", menuName = "SO/GameEntity/UnitFeedingConfigsSO")]
    public class UnitFeedingConfigsSO : ScriptableObject
    {
        public const string DefaultAssetPath = "Misc/UnitFeedingConfigsSO";
        public UnitFeedingConfigs UnitFeedingConfigs;
    }
}