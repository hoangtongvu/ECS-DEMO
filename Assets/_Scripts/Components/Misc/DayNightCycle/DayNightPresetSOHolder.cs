using Core.Misc.DayNightCycle;
using Unity.Entities;

namespace Components.Misc.DayNightCycle
{
    public struct DayNightPresetSOHolder : IComponentData
    {
        public UnityObjectRef<DayNightPresetSO> Value;
    }
}
