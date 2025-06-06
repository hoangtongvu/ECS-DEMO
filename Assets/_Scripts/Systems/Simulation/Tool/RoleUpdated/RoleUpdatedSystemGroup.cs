using Unity.Entities;

namespace Systems.Simulation.Tool.RoleUpdated
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(ToolPickSystem))]
    public partial class RoleUpdatedSystemGroup : ComponentSystemGroup
    {
	}
}