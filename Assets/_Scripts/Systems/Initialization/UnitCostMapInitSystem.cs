using Components.Unit;
using Core.Unit;
using Core.GameResource;
using System;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Utilities;


namespace Systems.Initialization
{

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class UnitCostMapInitSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.LoadUnitProfileSOs(out var unitProfiles);

            int length = unitProfiles.Length;
            int initialCap = length * Enum.GetNames(typeof(ResourceType)).Length;

            var unitCostMap = new UnitCostMap
            {
                Value = new(initialCap, Allocator.Persistent),
            };

            for (int i = 0; i < length; i++)
            {
                var profile = unitProfiles[i];

                foreach (var cost in profile.BaseCosts)
                {
                    var unitCostId = new UnitCostId(profile.UnitType, cost.Key, profile.LocalIndex);
                    if (unitCostMap.Value.TryAdd(unitCostId, cost.Value))
                    {
                        // Debug.Log($"UnitCostMap added UnitCostId: {unitCostId}");
                        continue;
                    }
                    Debug.LogError($"UnitCostMap already contains UnitCostId: {unitCostId}");
                }
            }


            SingletonUtilities.GetInstance(EntityManager)
                .AddOrSetComponentData(unitCostMap);


        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
        }

        private void LoadUnitProfileSOs(out UnitProfileSO[] unitProfiles) => unitProfiles = Resources.LoadAll<UnitProfileSO>("");
    }
}