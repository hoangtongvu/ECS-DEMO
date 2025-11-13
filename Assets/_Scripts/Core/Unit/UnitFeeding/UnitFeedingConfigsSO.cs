using SOConstantsGenerator;
using UnityEngine;

namespace Core.Unit.UnitFeeding
{
    [GenerateConstantsFor("UnitFeedingConfigConstants", "Core.Unit.UnitFeeding")]
    [CreateAssetMenu(fileName = "UnitFeedingConfigsSO", menuName = "SO/GameEntity/UnitFeedingConfigsSO")]
    public partial class UnitFeedingConfigsSO : ScriptableObject
    {
        public const string DefaultAssetPath = "Misc/UnitFeedingConfigsSO";
        [ConstantField] public UnitFeedingConfigs UnitFeedingConfigs;
    }
}