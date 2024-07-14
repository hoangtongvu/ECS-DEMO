using UnityEngine;

namespace Core.UI.EntitySpawningPanel
{
    public class EntitySpawningPanelCtrl : BaseUICtrl
    {
        [SerializeField] private SpawningDisplaysHolder spawningDisplaysHolder;

        public SpawningDisplaysHolder SpawningDisplaysHolder => spawningDisplaysHolder;


        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.spawningDisplaysHolder);
        }
    }
}