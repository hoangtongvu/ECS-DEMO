using Components.GameEntity.EntitySpawning;
using Unity.Entities;
using UnityEngine;

namespace Authoring.GameEntity.EntitySpawning
{
    public class SpawnerPosAuthoring : MonoBehaviour
    {

        private class Baker : Baker<SpawnerPosAuthoring>
        {
            public override void Bake(SpawnerPosAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<SpawnerPos>(entity);
            }
        }
    }
}
