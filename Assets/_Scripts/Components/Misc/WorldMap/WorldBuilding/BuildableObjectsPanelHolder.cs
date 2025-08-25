using Core.UI.WorldMap.BuildableObjects.BuildableObjectsPanel;
using Unity.Entities;

namespace Components.Misc.WorldMap.WorldBuilding
{
    public struct BuildableObjectsPanelHolder : IComponentData
    {
        public UnityObjectRef<BuildableObjectsPanelCtrl> Value;
    }
}
