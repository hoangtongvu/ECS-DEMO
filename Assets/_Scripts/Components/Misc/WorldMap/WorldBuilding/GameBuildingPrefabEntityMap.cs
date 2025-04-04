using Core.Misc.WorldMap.WorldBuilding;
using Unity.Collections;
using Unity.Entities;

namespace Components.Misc.WorldMap.WorldBuilding
{
    public struct GameBuildingPrefabEntityMap : IComponentData
    {
        public NativeHashMap<GameBuildingProfileId, Entity> Value;
    }

}
