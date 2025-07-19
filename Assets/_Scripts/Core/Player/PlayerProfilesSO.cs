using AYellowpaper.SerializedCollections;
using Core.GameEntity;
using Core.Misc.WorldMap.WorldBuilding;
using Core.Player.Reaction;
using UnityEngine;

namespace Core.Player
{
    [System.Serializable]
    public class PlayerProfileElement : GameEntityProfileElement
    {
        public PlayerReactionConfigs PlayerReactionConfigs = new();
        public SerializedDictionary<GameBuildingProfileId, bool> PlayerBuildingIds; // I don't have serialized hash set yet so I will use bool as the value placeholder.
    }

    [CreateAssetMenu(fileName = "PlayerProfilesSO", menuName = "SO/GameEntity/PlayerProfilesSO")]
    public class PlayerProfilesSO : GameEntityProfilesSO<PlayerProfileId, PlayerProfileElement>
    {
        public static readonly string DefaultAssetPath = "Misc/PlayerProfilesSO";
    }

}