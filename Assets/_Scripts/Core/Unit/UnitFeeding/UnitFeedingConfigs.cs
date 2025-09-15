using System;

namespace Core.Unit.UnitFeeding;

[Serializable]
public struct UnitFeedingConfigs
{
    public FeedingEventConfigs FeedingEventConfigs;
    public HungerBarConfigs HungerBarConfigs;
}
