using Core.GameResource;
using Unity.Entities;

namespace Components.GameResource
{
    public struct ResourceItemEntityHolder : IComponentData
    {
        public Entity Value;
    }


    public struct ResourceItemICD : IComponentData
    {
        public ResourceType ResourceType;
        public uint Quantity;
    }

}
