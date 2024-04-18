using Components.ComponentMap;
using Core.UI.Identification;
using System.Collections.Generic;
using Unity.Entities;


namespace Systems.Initialization
{

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class UIMapInitSystem : SystemBase
    {
        protected override void OnCreate()
        {
            Entity entity = EntityManager.CreateEntity();

            EntityManager.AddComponent<UIPoolMap>(entity);
            EntityManager.SetComponentData(entity, new UIPoolMap
            {
                Value = new Dictionary<UIType, UIPoolMapValue>(),
            });

            EntityManager.AddComponent<SpawnedUIMap>(entity);
            EntityManager.SetComponentData(entity, new SpawnedUIMap
            {
                Value = new Dictionary<UIID, Core.UI.BaseUICtrl>(),
            });

            EntityManager.SetName(entity, "UIMap");

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
        }
    }
}