using Core.GameResource;
using System.Collections.Generic;
using Unity.Entities;

namespace Components.GameResource
{
    public struct GameResourceElement : IBufferElementData
    {
        public Core.GameResource.GameResource Value;
    }

    public class GameResourceMap : IComponentData
    {
        public Dictionary<ResourceType, uint> Value;
    }

}
