using Unity.Entities;

namespace Components.Harvest
{
    public struct HarvestTargetEntity : IComponentData
    {
        public Entity Value;
    }
}
