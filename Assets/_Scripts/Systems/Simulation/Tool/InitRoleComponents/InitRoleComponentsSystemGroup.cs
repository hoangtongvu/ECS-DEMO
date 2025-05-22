using Unity.Entities;

namespace Systems.Simulation.Tool.InitRoleComponents
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(ToolPickSystem))]
    public partial class InitRoleComponentsSystemGroup : ComponentSystemGroup
    {
	}
}