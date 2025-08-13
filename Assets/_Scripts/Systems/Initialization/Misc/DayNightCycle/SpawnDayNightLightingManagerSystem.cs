using Components.Misc.DayNightCycle;
using Core.Misc.DayNightCycle;
using Unity.Entities;
using UnityEngine;

namespace Systems.Initialization.Misc.DayNightCycle
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class SpawnDayNightLightingManagerSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<DayNightPresetSOHolder>();
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var preset = SystemAPI.GetSingleton<DayNightPresetSOHolder>().Value.Value;

            var go = new GameObject($"*{nameof(DayNightLightingManager)}");
            var dayNightLightingManager = go.AddComponent<DayNightLightingManager>();

            dayNightLightingManager.Preset = preset;

        }

    }

}