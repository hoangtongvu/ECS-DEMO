using AYellowpaper.SerializedCollections;
using Core.GameEntity;
using Core.GameResource;
using UnityEngine;

namespace Core.Misc.WorldMap.WorldBuilding
{
    [System.Serializable]
    public class GameBuildingProfileElement : GameEntityProfileElement
    {
        [Header("GameBuilding ProfileElement")]
        public uint MaxHp;

        [SerializedDictionary("ResourceType ", "Quantity")]
        public SerializedDictionary<ResourceType, uint> BaseSpawningCosts;
    }

    [CreateAssetMenu(fileName = "GameBuildingProfilesSO", menuName = "SO/GameEntity/GameBuildingProfilesSO")]
    public class GameBuildingProfilesSO : GameEntityProfilesSO<GameBuildingProfileId, GameBuildingProfileElement>
    {
        public static readonly string DefaultAssetPath = "Misc/GameBuildingProfilesSO";
    }

}