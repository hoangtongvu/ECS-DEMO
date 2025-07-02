using System;

namespace Core.Misc
{
    [Flags]
    public enum CollisionLayer // TODO: Auto gen CollisionLayer based on TagManager.asset
    {
        Default = 1 << 0,
        Player = 1 << 6,
        Ground = 1 << 7,
        Unit = 1 << 8,
        Item = 1 << 9,
        Building = 1 << 10,
        Tool = 1 << 11,
    }
}