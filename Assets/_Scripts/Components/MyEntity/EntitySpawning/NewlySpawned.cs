using Unity.Entities;
using Unity.Mathematics;

namespace Components.MyEntity.EntitySpawning
{
    public struct NewlySpawnedTag : IComponentData, IEnableableComponent
    {
    }

    public struct SpawnerPos : IComponentData
    {
        public float3 Value;
    }

    public struct SpawnerEntityRef : IComponentData
    {
        public Entity Value;
    }

}
