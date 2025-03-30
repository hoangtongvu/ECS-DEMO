using Components.GameEntity.EntitySpawning;
using Unity.Entities;
using UnityEngine;

namespace Authoring.GameEntity.EntitySpawning
{
    public class SpawnerEntityRefAuthoring : MonoBehaviour
    {

        private class Baker : Baker<SpawnerEntityRefAuthoring>
        {
            public override void Bake(SpawnerEntityRefAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<SpawnerEntityRef>(entity);
            }
        }
    }
}
