using UnityEngine;

namespace Core.Misc.WorldMap.WorldBuilding
{
    [CreateAssetMenu(fileName = "BuildableObjectsSO", menuName = "SO/Misc/WorldMap/WorldBuilding/BuildableObjectsSO")]
    public class BuildableObjectsSO : ScriptableObject
    {
        public static string DefaultAssetPath = "Misc/WorldMap/WorldBuilding/BuildableObjectsSO";
        public BuildableObjectSOElement[] BuildableObjects;
    }

}
