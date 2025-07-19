
namespace Core.Player.Reaction
{
    [System.Serializable]
    public struct PlayerReactionConfigs
    {
        public float WalkSpeed = 5f;
        public float RunSpeed = 8f;

        public float AccelerationValue = 12f;
        public float DecelerationValue = 20f;

        public PlayerReactionConfigs() { }
    }

}
