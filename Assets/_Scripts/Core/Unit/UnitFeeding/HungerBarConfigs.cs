using System;

namespace Core.Unit.UnitFeeding;

[Serializable]
public struct HungerBarConfigs
{
    public float HungerDrainSpeed;
    public float HungerValuePerFood;
    public float HungerBarCap;
    public StarvingThresholdConfigs StarvingThresholdConfigs;
    public NormalThresholdConfigs NormalThresholdConfigs;
    public FullThresholdConfigs FullThresholdConfigs;
}

[Serializable]
public struct StarvingThresholdConfigs
{
    public float ThresholdUpperBound;
    public float StarvingTakeDmgIntervalMinutes;
    public uint BaseDmgTakenPerInterval;
    public float QuadraticCoefficientDmgTakenPerInterval;
}

[Serializable]
public struct NormalThresholdConfigs
{
    public float ThresholdUpperBound;
}

[Serializable]
public struct FullThresholdConfigs
{
    public float HpRegenIntervalSeconds;
    public uint HpRegenValuePerInterval;
}
