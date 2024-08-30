using Components.Tool;
using Core.Tool;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Utilities;


namespace Systems.Initialization
{

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class Tool2UnitMapInitSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.LoadSO(out var tool2UnitMapSO);


            var tool2UnitMap = new Tool2UnitMap
            {
                Value = new NativeHashMap<byte, byte>(15, Allocator.Persistent),
            };

            foreach (var item in tool2UnitMapSO.Map)
            {
                var keyAsByte = (byte)item.Key;
                var valueAsByte = (byte)item.Value;

                if (tool2UnitMap.Value.TryAdd(keyAsByte, valueAsByte))
                {
                    //Debug.Log($"Added {keyAsByte} - {valueAsByte}");
                    continue;
                }

                Debug.LogError($"Tool2UnitMap already contains {item.Key}");
            }


            SingletonUtilities.GetInstance(EntityManager)
                .AddOrSetComponentData(tool2UnitMap);

            this.Enabled = false;
        }

        protected override void OnUpdate()
        {
        }

        private void LoadSO(out Tool2UnitMapSO so) => so = Resources.Load<Tool2UnitMapSO>("Tool2UnitMap");
    }
}