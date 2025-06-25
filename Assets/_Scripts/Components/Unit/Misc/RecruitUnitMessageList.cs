using Core.Unit.Misc;
using Unity.Collections;
using Unity.Entities;

namespace Components.Unit.Misc
{
    public struct RecruitUnitMessageList : IComponentData
    {
        public NativeList<RecruitUnitMessage> Value;
    }

}
