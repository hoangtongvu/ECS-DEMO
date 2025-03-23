using UnityEngine;
using System.Collections.Generic;

namespace Core.MyEntity
{
    public abstract class EntitySpawningProfilesSO : ScriptableObject
    {
        public abstract IEnumerable<EntityProfileSO> GetProfiles();

    }

}