using Core.Misc;
using Core.UI.EntitySpawningPanel.SpawningProfileDisplay;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI.EntitySpawningPanel
{
    public class SpawningDisplaysHolder : SaiMonoBehaviour
    {
        [SerializeField] private List<SpawningProfileDisplayCtrl> value;

        public List<SpawningProfileDisplayCtrl> Value => value;

        public void Add(SpawningProfileDisplayCtrl profileDisplayCtrl)
        {
            this.value.Add(profileDisplayCtrl);
            profileDisplayCtrl.transform.SetParent(transform);
        }

    }

}