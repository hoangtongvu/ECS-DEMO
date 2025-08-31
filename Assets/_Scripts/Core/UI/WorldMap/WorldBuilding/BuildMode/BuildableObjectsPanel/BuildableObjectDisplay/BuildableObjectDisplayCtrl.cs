using Core.UI.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel.BuildableObjectDisplay;
using Core.UI.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel.BuildableObjectDisplay.Previews;
using Core.UI.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel.BuildableObjectDisplay.Previews.PreviewImage;
using UnityEngine;

namespace Core.UI.WorldMap.BuildableObjects.BuildableObjectsPanel.BuildableObjectDisplay
{
    [GenerateUIType("BuildableObjectDisplay")]
    public partial class BuildableObjectDisplayCtrl : BaseUICtrl
    {
        [SerializeField] private DisplayButton displayButton;
        [SerializeField] private PreviewsCtrl previewsCtrl;
        public int IndexInDisplaysHolder;

        public PreviewImageCtrl DisplayPreviewImage => previewsCtrl.PreviewImage;
        public PreviewsCtrl PreviewsCtrl => previewsCtrl;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInCtrl(ref this.displayButton);
            this.LoadComponentInChildren(ref this.previewsCtrl);
        }

        public override void OnRent()
        {
            this.previewsCtrl.OnRent();
        }

        public override void OnReturn()
        {
            this.previewsCtrl.OnReturn();
        }

    }

}
