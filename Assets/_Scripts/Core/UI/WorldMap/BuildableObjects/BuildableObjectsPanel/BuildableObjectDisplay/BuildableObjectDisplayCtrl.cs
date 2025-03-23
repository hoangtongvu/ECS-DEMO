using UnityEngine;

namespace Core.UI.WorldMap.BuildableObjects.BuildableObjectsPanel.BuildableObjectDisplay
{
    [GenerateUIType("BuildableObjectDisplay")]
    public partial class BuildableObjectDisplayCtrl : BaseUICtrl
    {
        [SerializeField] private DisplayButton displayButton;
        [SerializeField] private DisplayPreviewImage previewImage;
        [SerializeField] private DisplayBuildName buildNameText;
        public int IndexInDisplaysHolder;

        public DisplayPreviewImage DisplayPreviewImage => previewImage;
        public DisplayBuildName BuildNameText => buildNameText;

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.LoadComponentInCtrl(ref this.displayButton);
            this.LoadComponentInChildren(ref this.previewImage);
            this.LoadComponentInChildren(ref this.buildNameText);
        }

    }

}
