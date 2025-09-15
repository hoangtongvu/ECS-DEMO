using System;

namespace Core.Unit.UnitFeeding;

[Serializable]
public struct FeedingEventConfigs
{
    public float FeedingIntervalMinutes;
    public uint FoodPerUnitPerFeedingEvent;
}
