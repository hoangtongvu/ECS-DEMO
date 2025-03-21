using Components.Misc.WorldMap.WorldBuilding.PlacementPreview;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Misc.WorldMap.WorldBuilding
{
    public class PlacementPreviewSpriteTagAuthoring : MonoBehaviour
    {
        private class Baker : Baker<PlacementPreviewSpriteTagAuthoring>
        {
            public override void Bake(PlacementPreviewSpriteTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent<PlacementPreviewSpriteTag>(entity);

            }

        }

    }

}
