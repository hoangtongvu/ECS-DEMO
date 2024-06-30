using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Core.GameResource
{
    [System.Serializable]
    public class ResourceProfile
    {
        public Sprite ResourceIcon;
    }


    [CreateAssetMenu(fileName = "ResourceProfilesManager", menuName = "SO/Misc/ResourceProfilesManager")]
    public class ResourceProfilesManagerSO : ScriptableObject
    {
        [SerializedDictionary("ResourceType", "Profile")]
        public SerializedDictionary<ResourceType, ResourceProfile> Profiles;

    }
}