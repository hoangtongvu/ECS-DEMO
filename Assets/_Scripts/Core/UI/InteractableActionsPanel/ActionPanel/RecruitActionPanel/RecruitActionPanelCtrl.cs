using Core.UI.Identification;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI.InteractableActionsPanel.ActionPanel.RecruitActionPanel
{
    [GenerateUIType("ActionPanel_Recruit")]
    public partial class RecruitActionPanelCtrl : ActionPanelCtrl
    {
        [SerializeField]
        private RecruitActionCostViewsHolder recruitActionCostViewsHolder;

        public RecruitActionCostViewsHolder RecruitActionCostViewsHolder => recruitActionCostViewsHolder;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.recruitActionCostViewsHolder);
        }

        public override void Despawn(Dictionary<UIType, UIPrefabAndPool> uiPrefabAndPoolMap, Dictionary<UIID, BaseUICtrl> spawnedUIMap)
        {
            base.Despawn(uiPrefabAndPoolMap, spawnedUIMap);

            foreach (var recruitActionCostView in this.recruitActionCostViewsHolder.Value)
            {
                recruitActionCostView.Despawn(uiPrefabAndPoolMap, spawnedUIMap);
            }

            this.recruitActionCostViewsHolder.Value.Clear();

        }

    }

}