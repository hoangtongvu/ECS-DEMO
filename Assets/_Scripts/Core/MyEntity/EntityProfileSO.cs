using AYellowpaper.SerializedCollections;
using Core.GameResource;
using UnityEngine;

namespace Core.MyEntity
{
    public abstract class EntityProfileSO : ScriptableObject
    {
        [Header("Entity Profile")]
        public ushort LocalIndex;

        public Sprite ProfilePicture;
        public string UnitName;
        public GameObject Prefab;
        public float DurationPerSpawn;

        [SerializedDictionary("ResourceType ", "Quantity")]
        public SerializedDictionary<ResourceType, uint> BaseCosts;
    }
}