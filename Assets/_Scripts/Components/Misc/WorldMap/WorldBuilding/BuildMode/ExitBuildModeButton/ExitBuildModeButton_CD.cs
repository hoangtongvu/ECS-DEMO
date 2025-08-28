using Core.UI.WorldMap.BuildableObjects.ExitBuildModeButton;
using Unity.Entities;

namespace Components.Misc.WorldMap.WorldBuilding.BuildMode.ExitBuildModeButton
{
    public struct ExitBuildModeButton_CD
    {
        public struct Holder : IComponentData
        {
            public UnityObjectRef<ExitBuildModeButtonCtrl> Value;
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
