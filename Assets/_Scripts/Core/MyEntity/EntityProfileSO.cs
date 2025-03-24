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
        public string Name;
        public GameObject Prefab;
        public float SpawnDurationSeconds;

        [SerializedDictionary("ResourceType ", "Quantity")]
        public SerializedDictionary<ResourceType, uint> BaseCosts;

    }

}