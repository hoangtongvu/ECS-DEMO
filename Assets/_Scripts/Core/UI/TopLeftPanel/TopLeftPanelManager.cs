using Core.UI.Identification;
using Core.UI.TopLeftPanel.ResourceDisplay;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI.TopLeftPanel
{
    [GenerateUIType("TopLeftPanel")]
    public partial class TopLeftPanelManager : BaseUICtrl
    {
        [SerializeField] private List<ResourceDisplayCtrl> resourceDisplayCtrls;

        public void AddResourceDisplay(ResourceDisplayCtrl resourceDisplayCtrl)
        {
            this.resourceDisplayCtrls.Add(resourceDisplayCtrl);
            resourceDisplayCtrl.transform.SetParent(this.transform);
        }

        public void RemoveResourceDisplay(UIID uiId)
        {
            foreach (var resourceDisplayCtrl in this.resourceDisplayCtrls)
            {
                if (resourceDisplayCtrl.RuntimeUIID.Equals(uiId)) continue;
                this.resourceDisplayCtrls.Remove(resourceDisplayCtrl);
                break;
            }

        }

        public override void OnRent()
        {
        }

        public override void OnReturn()
        {
        }

    }

}