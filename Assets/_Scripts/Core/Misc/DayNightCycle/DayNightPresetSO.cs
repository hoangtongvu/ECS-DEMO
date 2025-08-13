using UnityEngine;

namespace Core.Misc.DayNightCycle
{
    [CreateAssetMenu(fileName = "DayNightPresetSO", menuName = "SO/Misc/DayNightPresetSO")]
    public class DayNightPresetSO : ScriptableObject
    {
        public Gradient AmbientColor;
        public Gradient DirectionalColor;
        public Gradient FogColor;

        [Range(0, 24)]
        public float StartHourInTheDay = 7f;

        public float MinutesPerDayCycle = 7f;
    }
}