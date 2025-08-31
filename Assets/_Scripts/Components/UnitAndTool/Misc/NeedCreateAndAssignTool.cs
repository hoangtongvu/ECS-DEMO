using Core.Tool;
using Unity.Entities;

namespace Components.UnitAndTool.Misc
{
    public struct NeedCreateAndAssignTool : IComponentData
    {
        public ToolProfileId Value;
    }
}
