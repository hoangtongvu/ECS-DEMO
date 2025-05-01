namespace Core.Misc.GlobalConfigs
{
    [System.Serializable]
    public struct GameGlobalConfigs
    {
        public float UnitIdleMaxDuration = 3f;
        public float UnitWalkMinDistance = 2f;
        public float UnitWalkMaxDistance = 5f;

        public GameGlobalConfigs() { }
    }

}
