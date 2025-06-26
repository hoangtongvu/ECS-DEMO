using Core.Misc;
using Core.UI.InteractableActionsPanel.ActionPanel;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI.InteractableActionsPanel
{
    public class ActionPanelsHolder : SaiMonoBehaviour
    {
        [SerializeField] private List<ActionPanelCtrl> value;

        public List<ActionPanelCtrl> Value => value;

        public void Add(ActionPanelCtrl newCtrl)
        {
            int listCount = this.value.Count;

            for (int i = 0; i < listCount; i++)
            {
                var actionPanelCtrl = this.value[i];
                if (newCtrl.PriorityInContainer > actionPanelCtrl.PriorityInContainer) continue;

                this.value.Insert(i, newCtrl);
                newCtrl.transform.SetParent(transform);
                newCtrl.transform.SetSiblingIndex(i);
                return;
            }

            this.value.Add(newCtrl);
            newCtrl.transform.SetParent(transform);
        }

    }

}