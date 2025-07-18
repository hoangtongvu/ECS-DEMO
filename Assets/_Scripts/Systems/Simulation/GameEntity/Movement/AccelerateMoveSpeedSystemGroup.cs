using Unity.Entities;

namespace Systems.Simulation.GameEntity.Movement
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(SetTargetMoveSpeedSystemGroup))]
    public partial class AccelerateMoveSpeedSystemGroup : ComponentSystemGroup
    {
    }
}
