using Unity.Entities;
using Unity.Burst;
using Components;
using Components.Damage;
using Components.Unit;
using Unity.Mathematics;
using Unity.Transforms;
using Utilities.Helpers;
using Components.Unit.MyMoveCommand;
using Core.Unit.MyMoveCommand;

namespace Systems.Simulation.Unit
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct UnitWalkSystem : ISystem
    {
        private Random rand;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.rand = new(1);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var moveCommandSourceMap = SystemAPI.GetSingleton<MoveCommandSourceMap>();

            foreach (var (unitIdRef, walkSpeedRef, moveSpeedRef, transformRef, walkMinDisRef, walkMaxDisRef, moveCommandElement, entity) in
                SystemAPI.Query<
                    RefRO<UnitId>
                    , RefRO<WalkSpeed>
                    , RefRW<MoveSpeedLinear>
                    , RefRW<LocalTransform>
                    , RefRO<WalkMinDistance>
                    , RefRO<WalkMaxDistance>
                    , RefRW<MoveCommandElement>>()
                    .WithAll<IsAliveTag>()
                    .WithAll<NeedsInitWalkTag>()
                    .WithEntityAccess())
            {

                SystemAPI.SetComponentEnabled<NeedsInitWalkTag>(entity, false);

                bool canOverrideCommand =
                    MoveCommandHelper.TryOverrideMoveCommand(
                        in moveCommandSourceMap.Value
                        , unitIdRef.ValueRO.UnitType
                        , ref moveCommandElement.ValueRW
                        , MoveCommandSource.PlayerCommand // Add a new Source for Walk or not this would override tool call
                        , unitIdRef.ValueRO.LocalIndex);

                if (!canOverrideCommand) continue;

                // Get a random direction
                float2 randomDir = this.rand.NextFloat2Direction();

                // Generate a random distance between the min and max distance
                float randomDis = this.rand.NextFloat(walkMinDisRef.ValueRO.Value, walkMaxDisRef.ValueRO.Value);

                float2 tempVector2 = randomDir * randomDis;

                // Calculate the random target position
                float3 randomPoint = transformRef.ValueRO.Position + new float3(tempVector2.x, 0, tempVector2.y);

                // Use this randomPoint for the move command or other logic
                moveCommandElement.ValueRW.Float3 = randomPoint;

                // Set move speed
                moveSpeedRef.ValueRW.Value = walkSpeedRef.ValueRO.Value;

                // SystemAPI.Query only supports upto 7 parameters...
                SystemAPI.SetComponent(entity, new TargetPosition
                {
                    Value = randomPoint,
                });
                SystemAPI.SetComponentEnabled<CanMoveEntityTag>(entity, true);
            }


        }




    }
}