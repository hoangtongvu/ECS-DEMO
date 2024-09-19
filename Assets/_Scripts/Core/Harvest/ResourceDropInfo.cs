using Core.GameResource;

namespace Core.Harvest
{
    [System.Serializable]
    public struct ResourceDropInfo
    {
        public ResourceType ResourceType;
        public uint QuantityPerDrop;
        public uint HpAmountPerDrop;
    }
}