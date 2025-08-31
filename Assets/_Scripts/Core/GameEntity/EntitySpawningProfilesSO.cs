using UnityEngine;
using System.Collections.Generic;

namespace Core.GameEntity
{
    [System.Serializable]
    public class EntitySpawningProfileElement
    {
        public GameObject PrefabToSpawn;
        public ushort AutoSpawnChancePerTenThousand;
    }

    [CreateAssetMenu(fileName = "EntitySpawningProfilesSO", menuName = "SO/GameEntity/EntitySpawningProfilesSO")]
    public class EntitySpawningProfilesSO : ScriptableObject
    {
        public bool UseAutoSpawnChances;
        public List<EntitySpawningProfileElement> Profiles;
    }

}