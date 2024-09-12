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
using Components.Misc.GlobalConfigs;

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

            state.RequireForUpdate<MoveCommandSourceMap>();

            EntityQuery query = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitId
                    , MoveSpeedLinear
                    , LocalTransform
                    , MoveCommandElement
                    , IsAliveTag
                    , NeedsInitWalkTag>()
                .Build();

            state.RequireForUpdate(query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var gameGlobalConfigs = SystemAPI.GetSingleton<GameGlobalConfigsICD>();
            var moveCommandSourceMap = SystemAPI.GetSingleton<MoveCommandSourceMap>();

            foreach (var (unitIdRef, moveSpeedRef, transformRef, moveCommandElement, entity) in
                SystemAPI.Query<
                    RefRO<UnitId>
                    , RefRW<MoveSpeedLinear>
                    , RefRW<LocalTransform>
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
                float randomDis = this.rand.NextFloat(gameGlobalConfigs.Value.UnitWalkMinDistance, gameGlobalConfigs.Value.UnitWalkMaxDistance);

                float2 tempVector2 = randomDir * randomDis;

                // Calculate the random target position
                float3 randomPoint = transformRef.ValueRO.Position + new float3(tempVector2.x, 0, tempVector2.y);

                // Use this randomPoint for the move command or other logic
                moveCommandElement.ValueRW.Float3 = randomPoint;

                // Set move speed
                moveSpeedRef.ValueRW.Value = gameGlobalConfigs.Value.UnitWalkSpeed;

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