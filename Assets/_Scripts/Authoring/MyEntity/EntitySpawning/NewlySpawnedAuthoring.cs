using Components.MyEntity.EntitySpawning;
using Unity.Entities;
using UnityEngine;

namespace Authoring.MyEntity.EntitySpawning
{
    public class NewlySpawnedAuthoring : MonoBehaviour
    {

        private class Baker : Baker<NewlySpawnedAuthoring>
        {
            public override void Bake(NewlySpawnedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<NewlySpawnedTag>(entity);

                // Set default position value for SpawnPos.
                AddComponent(entity, new SpawnPos
                {
                    Value = authoring.transform.position,
                });
            }
        }
    }
}
