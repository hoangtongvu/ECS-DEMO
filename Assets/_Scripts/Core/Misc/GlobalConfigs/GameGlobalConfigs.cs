namespace Core.Misc.GlobalConfigs
{

    [System.Serializable]
    public struct GameGlobalConfigs
    {
        public float EntityMoveSpeedScale = 1f;
        public float UnitStopMoveRadius = 1f;
        public float UnitInteractRadius = 1.5f;

        public float UnitWalkSpeed = 3f;
        public float UnitRunSpeed = 7f;

        public float UnitIdleMaxDuration = 3f;
        public float UnitWalkMinDistance = 2f;
        public float UnitWalkMaxDistance = 5f;

        public GameGlobalConfigs() { }
    }
}
