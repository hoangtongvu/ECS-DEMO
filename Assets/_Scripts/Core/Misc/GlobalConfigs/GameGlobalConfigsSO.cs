using UnityEngine;

namespace Core.Misc.GlobalConfigs
{

    [CreateAssetMenu(fileName = "GameGlobalConfigs", menuName = "SO/Misc/GlobalConfigs/GameGlobalConfigs")]
    public class GameGlobalConfigsSO : ScriptableObject
    {
        public static string DefaultAssetPath = "Misc/GlobalConfigs/GameGlobalConfigs";
        public GameGlobalConfigs Configs = new();
    }
}
