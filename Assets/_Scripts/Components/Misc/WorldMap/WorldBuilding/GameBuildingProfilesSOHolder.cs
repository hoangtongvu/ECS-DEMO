using Core.Misc.WorldMap.WorldBuilding;
using Unity.Entities;

namespace Components.Misc.WorldMap.WorldBuilding
{
    public struct GameBuildingProfilesSOHolder : IComponentData
    {
        public UnityObjectRef<GameBuildingProfilesSO> Value;
    }

}
