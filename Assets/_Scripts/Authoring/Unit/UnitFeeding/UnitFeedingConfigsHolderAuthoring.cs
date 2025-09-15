using Components.Unit.UnitFeeding;
using Core.Unit.UnitFeeding;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Unit.UnitFeeding
{
    public class UnitFeedingConfigsHolderAuthoring : MonoBehaviour
    {
        [SerializeField] private UnitFeedingConfigsSO configsSO;

        private class Baker : Baker<UnitFeedingConfigsHolderAuthoring>
        {
            public override void Bake(UnitFeedingConfigsHolderAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new UnitFeedingConfigsHolder
                {
                    Value = authoring.configsSO.UnitFeedingConfigs,
                });
            }

        }

    }

}
