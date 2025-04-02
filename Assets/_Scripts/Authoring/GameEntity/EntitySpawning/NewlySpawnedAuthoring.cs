using Components.GameEntity.EntitySpawning;
using Unity.Entities;
using UnityEngine;

namespace Authoring.GameEntity.EntitySpawning
{
    public class NewlySpawnedAuthoring : MonoBehaviour
    {

        private class Baker : Baker<NewlySpawnedAuthoring>
        {
            public override void Bake(NewlySpawnedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<NewlySpawnedTag>(entity);

            }
        }
    }
}
