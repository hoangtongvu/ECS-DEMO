using UnityEngine;
using System.Collections.Generic;

namespace Core.GameEntity
{
    [System.Serializable]
    public class EntitySpawningPrefabElement
    {
        public GameObject Value;

    }

    [CreateAssetMenu(fileName = "EntitySpawningPrefabsSO", menuName = "SO/GameEntity/EntitySpawningPrefabsSO")]
    public class EntitySpawningPrefabsSO : ScriptableObject
    {
        public List<EntitySpawningPrefabElement> Prefabs;

    }

}