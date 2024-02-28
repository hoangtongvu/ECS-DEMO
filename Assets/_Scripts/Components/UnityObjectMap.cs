using Components.CustomIdentification;
using System.Collections.Generic;
using Unity.Entities;

namespace Components
{
    public class UnityObjectMap : IComponentData
    {
        public Dictionary<UniqueId, UnityEngine.Object> Value;
    }
}