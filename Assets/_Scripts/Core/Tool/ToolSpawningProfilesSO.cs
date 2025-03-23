using UnityEngine;
using System.Collections.Generic;
using Core.MyEntity;

namespace Core.Tool
{
    [CreateAssetMenu(fileName = "ToolSpawningProfiles", menuName = "SO/MyEntity/ToolSpawningProfiles")]
    public class ToolSpawningProfilesSO : EntitySpawningProfilesSO
    {
        public List<ToolProfileSO> Profiles;

        public override IEnumerable<EntityProfileSO> GetProfiles() => this.Profiles;

    }

}