using Components.MyEntity.EntitySpawning;
using Unity.Entities;
using UnityEngine;

namespace Authoring.MyEntity.EntitySpawning
{
    public class SpawnerEntityAuthoring : MonoBehaviour
    {

        private class Baker : Baker<SpawnerEntityAuthoring>
        {
            public override void Bake(SpawnerEntityAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<SpawnerEntity>(entity);
            }
        }
    }
}
