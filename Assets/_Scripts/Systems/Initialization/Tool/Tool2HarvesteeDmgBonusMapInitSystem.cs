using Unity.Entities;
using Unity.Collections;
using Utilities;
using UnityEngine;
using Components.Tool;
using Core.Tool;

namespace Systems.Initialization.Tool
{

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class Tool2HarvesteeDmgBonusMapInitSystem : SystemBase
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

        private void LoadProfilesSO(out Tool2HarvesteeDmgBonusSO dmgBonusMapSO)
        {
            dmgBonusMapSO = Resources.Load<Tool2HarvesteeDmgBonusSO>(Tool2HarvesteeDmgBonusSO.DefaultAssetPath);
            if (dmgBonusMapSO != null) return;
            Debug.LogError($"Can't Load {nameof(Tool2HarvesteeDmgBonusSO)}");
        }

        private void CreateMap(Tool2HarvesteeDmgBonusSO dmgBonusMapSO)
        {
            var dmgBonusMap = new Tool2HarvesteeDmgBonusMap
            {
                Value = new(10, Allocator.Persistent),
            };

            foreach (var pair in dmgBonusMapSO.BonusMap)
            {
                if (dmgBonusMap.Value.TryAdd(pair.Key, pair.Value))
                {
                    continue;
                }

                Debug.LogError($"Can't add duplicated {nameof(ToolHarvesteePairId)} with Id = {pair.Key}");
            }

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(dmgBonusMap);
        }

    }
}