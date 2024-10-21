using Unity.Entities;

namespace Components.Unit
{
    public struct SelectedUnitMarkerTag : IComponentData
    {
    }
    
    public struct SelectedUnitMarkerPrefab : IComponentData
    {
        public Entity Value;
    }

    public struct SelectedUnitMarkerHolder : IComponentData
    {
        public Entity Value;
    }

}
