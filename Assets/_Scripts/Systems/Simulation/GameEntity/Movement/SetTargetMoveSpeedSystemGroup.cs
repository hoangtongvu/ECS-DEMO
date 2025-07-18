using Unity.Entities;

namespace Systems.Simulation.GameEntity.Movement
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class SetTargetMoveSpeedSystemGroup : ComponentSystemGroup
    {
    }
}
