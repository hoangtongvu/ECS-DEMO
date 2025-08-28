using Core.UI.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel;
using Unity.Entities;

namespace Components.Misc.WorldMap.WorldBuilding.BuildMode.BuildableObjectsPanel
{
    public struct BuildableObjectsPanel_CD
    {
        public struct Holder : IComponentData
        {
            public UnityObjectRef<BuildableObjectsPanelCtrl> Value;
        }

        public struct CanShow : IComponentData, IEnableableComponent
        {
        }

        public struct CanUpdate : IComponentData, IEnableableComponent
        {
        }

        public struct IsVisible : IComponentData, IEnableableComponent
        {
        }
    }
}
