using Unity.Entities;
using Unity.Mathematics;

namespace Components.MyEntity.EntitySpawning
{
    public struct NewlySpawnedTag : IComponentData, IEnableableComponent
    {
    }

    public struct SpawnPos : IComponentData
    {
        public float3 Value;
    }

}
