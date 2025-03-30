using UnityEngine;

namespace Core.MyEntity
{
    public abstract class EntityProfileSO : ScriptableObject
    {
        [Header("Entity Profile")]
        public ushort LocalIndex;
        public GameObject Prefab;

    }

}