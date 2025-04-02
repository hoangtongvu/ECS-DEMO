using AYellowpaper.SerializedCollections;
using Core.GameEntity;
using Core.GameResource;
using UnityEngine;

namespace Core.Misc.WorldMap.WorldBuilding
{
    [System.Serializable]
    public class PlayerBuildingProfileElement : GameEntityProfileElement
    {
        [Header("PlayerBuilding ProfileElement")]
        public uint MaxHp;

        [SerializedDictionary("ResourceType ", "Quantity")]
        public SerializedDictionary<ResourceType, uint> BaseSpawningCosts;
    }

    [CreateAssetMenu(fileName = "PlayerBuildingProfilesSO", menuName = "SO/GameEntity/PlayerBuildingProfilesSO")]
    public class PlayerBuildingProfilesSO : GameEntityProfilesSO<PlayerBuildingProfileId, PlayerBuildingProfileElement>
    {
        public static readonly string DefaultAssetPath = "Misc/PlayerBuildingProfilesSO";
    }

}