using Core.GameEntity;
using UnityEngine;

namespace Core.Player
{
    [System.Serializable]
    public class PlayerProfileElement : GameEntityProfileElement
    {
    }

    [CreateAssetMenu(fileName = "PlayerProfilesSO", menuName = "SO/GameEntity/PlayerProfilesSO")]
    public class PlayerProfilesSO : GameEntityProfilesSO<int, PlayerProfileElement>
    {
        public static readonly string DefaultAssetPath = "Misc/PlayerProfilesSO";
    }

}