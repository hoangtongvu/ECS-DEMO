using Core.Misc;
using Core.UI.InteractableActionsPanel.ActionPanel.RecruitActionPanel.RecruitActionCostView;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI.InteractableActionsPanel.ActionPanel.RecruitActionPanel
{
    public class RecruitActionCostViewsHolder : SaiMonoBehaviour
    {
        [SerializeField] private List<RecruitActionCostViewCtrl> value;

        public List<RecruitActionCostViewCtrl> Value => value;

        public void Add(RecruitActionCostViewCtrl newCtrl)
        {
            this.value.Add(newCtrl);
            newCtrl.transform.SetParent(transform);
        }

    }

}