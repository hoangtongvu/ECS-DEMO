using Systems.Simulation.Tool;
using Unity.Entities;

namespace Systems.Simulation.UnitAndTool.ToolPick
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(ToolAssignSystem))]
    public partial class ToolPickHandleSystemGroup : ComponentSystemGroup
    {
    }
}