using UnityEngine;

namespace Core.Misc.WorldMap
{
    [CreateAssetMenu(fileName = "ChunkDebugConfig", menuName = "SO/Misc/WorldMap/ChunkDebugConfig")]
    public class ChunkDebugConfigSO : ScriptableObject
    {
        public static string DefaultAssetPath = "Misc/WorldMap/ChunkDebugConfig";
        public Color[] ChunkGridLineColors;
    }
}