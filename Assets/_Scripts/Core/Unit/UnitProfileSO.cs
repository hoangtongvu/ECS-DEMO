using UnityEngine;

namespace Core.Unit
{
    [CreateAssetMenu(fileName = "UnitProfile", menuName = "SO/Unit")]
    public class UnitProfileSO : ScriptableObject
    {
        public Sprite ProfilePicture;
        public string UnitName;
        public GameObject Prefab;
        public float DurationPerUnit;
    }
}