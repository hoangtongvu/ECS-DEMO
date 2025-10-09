using Unity.Entities;
using Unity.Mathematics;

namespace Core.GameResource
{
    public struct ResourceItemSpawnCommand
    {
        public Entity SpawnerEntity; // Could be Dropper
        public float3 SpawnPos;
        public ResourceType ResourceType;
        public uint Quantity;
    }
}
