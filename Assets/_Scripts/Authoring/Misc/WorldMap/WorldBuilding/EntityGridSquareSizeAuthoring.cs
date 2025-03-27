using Components.Misc.WorldMap;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Misc.WorldMap.WorldBuilding
{
    public class EntityGridSquareSizeAuthoring : MonoBehaviour
    {
        [SerializeField] private int entityGridSquareSize;

        private class Baker : Baker<EntityGridSquareSizeAuthoring>
        {
            public override void Bake(EntityGridSquareSizeAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new EntityGridSquareSize
                {
                    Value = authoring.entityGridSquareSize,
                });

            }

        }

    }

}
