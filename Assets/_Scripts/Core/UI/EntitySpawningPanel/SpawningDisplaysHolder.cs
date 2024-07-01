using Core.UI.EntitySpawningPanel.SpawningProfileDisplay;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI.EntitySpawningPanel
{
    public class SpawningDisplaysHolder : SaiMonoBehaviour
    {
        [SerializeField] private List<SpawningProfileDisplayCtrl> spawningProfileDisplayCtrls;

        public void Add(SpawningProfileDisplayCtrl profileDisplayCtrl)
        {
            this.spawningProfileDisplayCtrls.Add(profileDisplayCtrl);
            profileDisplayCtrl.transform.SetParent(transform);
        }

    }
}