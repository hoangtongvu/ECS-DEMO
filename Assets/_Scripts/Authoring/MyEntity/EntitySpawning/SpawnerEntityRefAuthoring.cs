using Components.MyEntity.EntitySpawning;
using Unity.Entities;
using UnityEngine;

namespace Authoring.MyEntity.EntitySpawning
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
