using Core.Harvest;
using Unity.Entities;

namespace Components.Harvest
{
    public struct HarvesteeProfilesSOHolder : IComponentData
    {
        public UnityObjectRef<HarvesteeProfilesSO> Value;
    }

}
