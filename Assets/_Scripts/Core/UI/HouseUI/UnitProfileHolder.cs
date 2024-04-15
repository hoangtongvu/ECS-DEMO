using Core.UI.UnitProfile;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI.HouseUI
{
    public class UnitProfileHolder : SaiMonoBehaviour
    {
        [SerializeField] private List<UnitProfileUICtrl> unitProfileUICtrls;

        public void Add(UnitProfileUICtrl profileUICtrl)
        {
            this.unitProfileUICtrls.Add(profileUICtrl);
            profileUICtrl.transform.SetParent(transform);
        }

        public void DespawnAll()
        {
            foreach (var profileUICtrl in this.unitProfileUICtrls)
            {
                profileUICtrl.Despawner.DespawnObject();
            }
        }
    }
}