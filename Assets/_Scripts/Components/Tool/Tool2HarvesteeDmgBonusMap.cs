using Core.Tool;
using Unity.Collections;
using Unity.Entities;

namespace Components.Tool
{
    public struct Tool2HarvesteeDmgBonusMap : IComponentData
    {
        public NativeHashMap<ToolHarvesteePairId, float> Value;
    }
}
