using Core.Harvest;
using Unity.Collections;
using Unity.Entities;

namespace Components.Harvest
{
    public struct RangeInContainer
    {
        public int StartIndex;
        public int Count;
    }

    public struct HarvesteeProfileIdToPresenterVariancesRangeMap : IComponentData
    {
        public NativeHashMap<HarvesteeProfileId, RangeInContainer> Value;
    }

    public struct HarvesteePresenterVariancesContainer : IComponentData
    {
        public NativeList<Entity> Value;
    }

    public struct HarvesteePresenterVarianceTempBufferElement : IBufferElementData
    {
        public Entity Value;
    }

}
