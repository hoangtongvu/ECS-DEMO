using Core.UI.StructurePanel.UnitProfile;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI.StructurePanel
{
    public class UnitProfileUIHolder : SaiMonoBehaviour
    {
        [SerializeField] private List<UnitProfileUICtrl> unitProfileUICtrls;

        public void Add(UnitProfileUICtrl profileUICtrl)
        {
            this.unitProfileUICtrls.Add(profileUICtrl);
            profileUICtrl.transform.SetParent(transform);
        }

    }
}