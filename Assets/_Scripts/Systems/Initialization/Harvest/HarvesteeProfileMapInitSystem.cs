using Unity.Entities;
using Components.Harvest;
using Unity.Collections;
using Utilities;
using Core.Harvest;
using UnityEngine;

namespace Systems.Initialization.Harvest
{

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class HarvesteeProfileMapInitSystem : SystemBase
    {

        protected override void OnCreate()
        {
            this.LoadProfilesSO(out var harvesteeProfilesSO);
            this.CreateMap(harvesteeProfilesSO);

            this.Enabled = false;
        }

        protected override void OnUpdate()
        {
        }

        private void LoadProfilesSO(out HarvesteeProfilesSO harvesteeProfilesSO)
        {
            harvesteeProfilesSO = Resources.Load<HarvesteeProfilesSO>(HarvesteeProfilesSO.DefaultAssetPath);
            if (harvesteeProfilesSO != null) return;
            Debug.LogError($"Can't Load {nameof(HarvesteeProfilesSO)}");
        }

        private void CreateMap(HarvesteeProfilesSO harvesteeProfilesSO)
        {
            var harvesteeProfileMap = new HarvesteeProfileMap
            {
                Value = new(100, Allocator.Persistent),
            };

            foreach (var pair in harvesteeProfilesSO.Profiles)
            {
                if (harvesteeProfileMap.Value.TryAdd(pair.Key, pair.Value))
                {
                    continue;
                }

                Debug.LogError($"Can't add duplicated {nameof(HarvesteeProfileId)} with Id = {pair.Key}");
            }

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(harvesteeProfileMap);
        }

    }
}