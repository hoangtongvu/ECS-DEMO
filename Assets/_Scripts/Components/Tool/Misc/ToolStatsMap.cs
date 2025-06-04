using Core.Tool;
using Core.Tool.Misc;
using Unity.Collections;
using Unity.Entities;

namespace Components.Tool.Misc
{
    public struct ToolStatsMap : IComponentData
    {
        public NativeHashMap<ToolProfileId, ToolStats> Value;
    }

}
