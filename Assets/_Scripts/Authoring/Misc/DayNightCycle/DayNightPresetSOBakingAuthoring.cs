using Components.Misc.DayNightCycle;
using Core.Misc.DayNightCycle;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Misc.DayNightCycle
{
    public class DayNightPresetSOBakingAuthoring : MonoBehaviour
    {
        [SerializeField] private DayNightPresetSO presetSO;

        private class Baker : Baker<DayNightPresetSOBakingAuthoring>
        {
            public override void Bake(DayNightPresetSOBakingAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new DayNightPresetSOHolder
                {
                    Value = authoring.presetSO,
                });

            }

        }

    }

}
