using Components.GameEntity.EntitySpawning;
using Unity.Entities;
using UnityEngine;

namespace Authoring.GameEntity.EntitySpawning
{
    public class SpawnerAutoSpawnTagAuthoring : MonoBehaviour
    {
        private class Baker : Baker<SpawnerAutoSpawnTagAuthoring>
        {
            public override void Bake(SpawnerAutoSpawnTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<SpawnerAutoSpawnTag>(entity);
            }

        }

    }

}
