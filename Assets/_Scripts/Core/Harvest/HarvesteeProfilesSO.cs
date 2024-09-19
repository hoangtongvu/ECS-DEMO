using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Core.Harvest
{
    [CreateAssetMenu(fileName = "HarvesteeProfiles", menuName = "SO/Misc/HarvesteeProfilesSO")]
    public class HarvesteeProfilesSO : ScriptableObject
    {
        public static string DefaultAssetPath = "Misc/HarvesteeProfiles";

        [SerializedDictionary("ProfileId", "Profile")]
        public SerializedDictionary<HarvesteeProfileId, HarvesteeProfile> Profiles;
    }
}