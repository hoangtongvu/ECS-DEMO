using Components.CustomIdentification;
using System.Collections.Generic;
using Unity.Entities;

namespace Components
{
    public class UnityTransformMap : IComponentData
    {
        public Dictionary<UniqueId, UnityEngine.Transform> Value;
    }
}