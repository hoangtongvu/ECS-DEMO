using Core.GameEntity;
using UnityEngine;

namespace Core.GameResource
{
    [System.Serializable]
    public class ResourceProfileElement : GameEntityProfileElement
    {
        [Header("Resource ProfileElement")]
        public ResourceType ResourceType;
        public uint MaxQuantityPerStack;

    }

    [CreateAssetMenu(fileName = "ResourceProfilesSO", menuName = "SO/GameEntity/ResourceProfilesSO")]
    public class ResourceProfilesSO : GameEntityProfilesSO<ResourceProfileId, ResourceProfileElement>
    {
        public static readonly string DefaultAssetPath = "Misc/ResourceProfilesSO";
    }

}