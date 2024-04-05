using Components.Unit.UnitSpawning;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Unit.UnitSpawning
{
    public class NewlySpawnedAuthoring : MonoBehaviour
    {

        private class Baker : Baker<NewlySpawnedAuthoring>
        {
            public override void Bake(NewlySpawnedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<NewlySpawnedTag>(entity);
                AddComponent<SpawnPos>(entity);
            }
        }
    }
}
