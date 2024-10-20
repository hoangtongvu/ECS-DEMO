using Unity.Entities;

namespace Components.Unit
{
    public struct TargetPosMarkerTag : IComponentData
    {
    }
    
    public struct TargetPosMarkerPrefab : IComponentData
    {
        public Entity Value;
    }

    public struct TargetPosMarkerHolder : IComponentData
    {
        public Entity Value;
    }

}
