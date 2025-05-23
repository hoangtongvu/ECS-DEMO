
namespace Core.Unit.Reaction
{
    [System.Serializable]
    public struct UnitReactionConfigs
    {
        public float UnitWalkSpeed = 3f;
        public float UnitRunSpeed = 7f;

        public float UnitIdleMaxDuration = 3f;
        public float UnitWalkMinDistance = 2f;
        public float UnitWalkMaxDistance = 5f;

        public UnitReactionConfigs() { }
    }

}
