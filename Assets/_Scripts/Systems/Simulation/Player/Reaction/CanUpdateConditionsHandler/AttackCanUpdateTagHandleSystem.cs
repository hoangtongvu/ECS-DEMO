using Components.GameEntity.Attack;
using Components.GameEntity.Damage;
using Components.GameEntity.Reaction;
using Components.Misc;
using Components.Misc.WorldMap.WorldBuilding.PlacementPreview;
using Components.Player;
using DReaction;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Simulation.Player.Reaction.CanUpdateConditionsHandler
{
    [UpdateInGroup(typeof(CanUpdateConditionsHandleSystemGroup))]
    [BurstCompile]
    public partial struct AttackCanUpdateTagHandleSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    AttackReaction.CanUpdateTag
                    , AttackReaction.UpdatingTag
                    , AttackReaction.TimerSeconds>()
                .WithAll<
                    IsAliveTag>()
                .WithAll<
                    PlayerTag>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate<InputData>();
            state.RequireForUpdate<PlacementPreviewData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var placementPreviewData = SystemAPI.GetSingleton<PlacementPreviewData>();
            var inputData = SystemAPI.GetSingleton<InputData>();

            bool hardwareInputState = inputData.LeftMouseData.Down && ! inputData.IsPointerOverGameObject;

            foreach (var (reactionCanUpdateTag, reactionUpdatingTag, reactionTimerSecondsRef, attackDurationSecondsRef, isAliveTag) in SystemAPI
                .Query<
                    EnabledRefRW<AttackReaction.CanUpdateTag>
                    , EnabledRefRO<AttackReaction.UpdatingTag>
                    , RefRO<AttackReaction.TimerSeconds>
                    , RefRO<AttackDurationSeconds>
                    , EnabledRefRO<IsAliveTag>>()
                .WithAll<
                    PlayerTag>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                bool isInBuildMode = placementPreviewData.CanPlacementPreview;
                bool timedOut = reactionTimerSecondsRef.ValueRO.Value >= attackDurationSecondsRef.ValueRO.Value;

                reactionCanUpdateTag.ValueRW = isAliveTag.ValueRO && !isInBuildMode
                    && (hardwareInputState || (reactionUpdatingTag.ValueRO && !timedOut));
            }

        }

    }

}