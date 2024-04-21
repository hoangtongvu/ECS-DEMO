using UnityEngine;

namespace Core.UI.StructurePanel
{
    public class StructurePanelUICtrl : BaseUICtrl
    {
        [SerializeField] private UnitProfileUIHolder unitProfileUIHolder;

        public UnitProfileUIHolder UnitProfileUIHolder => unitProfileUIHolder;


        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.unitProfileUIHolder);
        }
    }
}