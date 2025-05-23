using Unity.Entities;

namespace Systems.Simulation.Tool.InitArmedStateComponents
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(ToolPickSystem))]
    public partial class InitArmedStateComponentsSystemGroup : ComponentSystemGroup
    {
	}
}