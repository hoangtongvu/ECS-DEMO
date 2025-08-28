using UnityEngine;

namespace Core.UI.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel
{
    [GenerateUIType("BuildableObjectsPanel")]
    public partial class BuildableObjectsPanelCtrl : BaseUICtrl
    {
        [SerializeField] private ObjectDisplaysHolder objectDisplaysHolder;

        public ObjectDisplaysHolder ObjectDisplaysHolder => objectDisplaysHolder;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInChildren(ref this.objectDisplaysHolder);
        }

        public override void OnRent()
        {
        }

        public override void OnReturn()
        {
            foreach (var displayCtrl in this.objectDisplaysHolder.Displays)
            {
                displayCtrl.ReturnSelfToPool();
            }

            this.objectDisplaysHolder.Displays.Clear();
        }

    }

}