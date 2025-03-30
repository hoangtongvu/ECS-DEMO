using UnityEngine;

namespace Core.MyEntity
{
    public abstract class EntityProfileSO : ScriptableObject
    {
        [Header("Entity Profile")]
        public GameObject Prefab;

    }

}