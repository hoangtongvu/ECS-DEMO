using Authoring.Utilities.Helpers.GameBuilding;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Misc.WorldMap.WorldBuilding
{
    public class GameBuildingAuthoring : MonoBehaviour
    {
        private class Baker : Baker<GameBuildingAuthoring>
        {
            public override void Bake(GameBuildingAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                GameBuildingBakingHelper.AddComponents(this, entity);
            }

        }

    }

}
