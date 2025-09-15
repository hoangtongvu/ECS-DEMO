using Unity.Entities;

namespace Systems.Simulation.Unit.UnitFeeding
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class HungerBarThresholdChangesHandleSystemGroup : ComponentSystemGroup
    {
    }
}