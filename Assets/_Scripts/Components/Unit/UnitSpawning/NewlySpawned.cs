using Unity.Entities;
using Unity.Mathematics;

namespace Components.Unit.UnitSpawning
{
    public struct NewlySpawnedTag : IComponentData, IEnableableComponent
    {
    }

    public struct SpawnPos : IComponentData
    {
        public float3 Value;
    }

}
