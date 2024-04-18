using System.Collections.Generic;
using Unity.Entities;

namespace Components.ComponentMap
{

    public class BaseMap<TKey, TValue> : IComponentData
    {
        public Dictionary<TKey, TValue> Value;
    }
}