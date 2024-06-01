using Components.ComponentMap;
using Core;
using Core.UI.Identification;
using System.Collections.Generic;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization
{

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class UIMapInitSystem : SystemBase
    {
        protected override void OnCreate()
        {

            SingletonUtilities.GetInstance(EntityManager)
                .AddOrSetComponentData(new UIPoolMap
                {
                    Value = new Dictionary<UIType, UIPoolMapValue>(),
                });

            SingletonUtilities.GetInstance(EntityManager)
                .AddOrSetComponentData(new SpawnedUIMap
                {
                    Value = new Dictionary<UIID, Core.UI.BaseUICtrl>(),
                });

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
        }
    }
}