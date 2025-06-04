using Unity.Mathematics;
using UnityEngine;

namespace Core.Tool.Misc
{
    [System.Serializable]
    public struct ToolStatsInSO
    {
        public uint BaseDmg;

        [Tooltip("Determines how many times unit can use their tool in 1 second")]
        public float BaseWorkSpeed;

        public ToolStatsInSO()
        {
            BaseDmg = 3;
            BaseWorkSpeed = 1.5f;
        }

        public readonly ToolStats ToToolStats()
        {
            return new ToolStats
            {
                BaseDmg = this.BaseDmg,
                BaseWorkSpeed = new(this.BaseWorkSpeed),
            };
        }

    }

    [System.Serializable]
    public struct ToolStats
    {
        public uint BaseDmg;
        public half BaseWorkSpeed;
    }

}