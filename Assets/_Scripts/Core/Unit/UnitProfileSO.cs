using AYellowpaper.SerializedCollections;
using Core.GameResource;
using UnityEngine;

namespace Core.Unit
{
    [CreateAssetMenu(fileName = "UnitProfile", menuName = "SO/Unit")]
    public class UnitProfileSO : ScriptableObject
    {
        public ushort LocalIndex;

        public Sprite ProfilePicture;
        public string UnitName;
        public UnitType UnitType;
        public GameObject Prefab;
        public float DurationPerUnit;

        [SerializedDictionary("ResourceType ", "Quantity")]
        public SerializedDictionary<ResourceType, uint> BaseCosts;
    }
}