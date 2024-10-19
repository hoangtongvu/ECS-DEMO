using Unity.Mathematics;

namespace Core.GameResource
{
    public struct ResourceItemSpawnCommand
    {
        public float3 SpawnPos;
        public ResourceType ResourceType;
        public uint Quantity;
    }

}
