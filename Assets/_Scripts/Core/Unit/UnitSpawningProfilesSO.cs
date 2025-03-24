using UnityEngine;
using System.Collections.Generic;
using Core.MyEntity;

namespace Core.Unit
{
    [CreateAssetMenu(fileName = "UnitSpawningProfiles", menuName = "SO/MyEntity/UnitSpawningProfiles")]
    public class UnitSpawningProfilesSO : EntitySpawningProfilesSO
    {
        public List<UnitProfileSO> Profiles;

        public override IEnumerable<EntityProfileSO> GetProfiles() => this.Profiles;

    }

}