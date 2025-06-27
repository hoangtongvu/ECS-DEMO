using Core.UI.Identification;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI.EntitySpawningPanel
{
    [GenerateUIType("EntitySpawningPanel")]
    public partial class EntitySpawningPanelCtrl : BaseUICtrl
    {
        [SerializeField] private SpawningDisplaysHolder spawningDisplaysHolder;

        public SpawningDisplaysHolder SpawningDisplaysHolder => spawningDisplaysHolder;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.spawningDisplaysHolder);
        }

        public override void Despawn(Dictionary<UIType, UIPrefabAndPool> uiPrefabAndPoolMap, Dictionary<UIID, BaseUICtrl> spawnedUIMap)
        {
            base.Despawn(uiPrefabAndPoolMap, spawnedUIMap);

            foreach (var spawningProfileDisplay in this.spawningDisplaysHolder.Value)
            {
                spawningProfileDisplay.Despawn(uiPrefabAndPoolMap, spawnedUIMap);
            }

            this.spawningDisplaysHolder.Value.Clear();

        }

    }

}