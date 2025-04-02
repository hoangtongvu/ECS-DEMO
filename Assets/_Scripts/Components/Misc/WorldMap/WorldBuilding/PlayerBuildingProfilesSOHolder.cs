using Core.Misc.WorldMap.WorldBuilding;
using Unity.Entities;

namespace Components.Misc.WorldMap.WorldBuilding
{
    public struct PlayerBuildingProfilesSOHolder : IComponentData
    {
        public UnityObjectRef<PlayerBuildingProfilesSO> Value;
    }

}
