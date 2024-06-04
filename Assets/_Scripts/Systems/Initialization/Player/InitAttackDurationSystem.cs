using Components.Player;
using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Components.Unit.UnitSpawning;

namespace Systems.Initialization.Player
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(AnimationClipInfoInitSystem))]
    [BurstCompile]
    public partial struct InitAttackDurationSystem : ISystem
    {
        private const string ATTACK_ANIM_NAME = "Punching";//TODO: Turn this into public Enum?

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            EntityQuery entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    AttackData
                    , AnimationClipInfoElement
                    , PlayerTag
                    , NewlySpawnedTag>()
                .Build();
            state.RequireForUpdate(entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            foreach (var (attackDataRef, clipInfos) in
                SystemAPI.Query<
                    RefRW<AttackData>
                    , DynamicBuffer<AnimationClipInfoElement>>()
                    .WithAll<PlayerTag>()
                    .WithAll<NewlySpawnedTag>())
            {
                attackDataRef.ValueRW.attackDurationSecond = this.GetAttackDuration(clipInfos, ATTACK_ANIM_NAME);
            }
        }

        [BurstCompile]
        private float GetAttackDuration(
            DynamicBuffer<AnimationClipInfoElement> clipInfos
            , FixedString64Bytes animName)  //TODO This seems a general logic, should move this to a Job/System/Static Function.
        {
            foreach (var clipInfo in clipInfos)
            {
                if (clipInfo.Name == animName) return clipInfo.Length;
            }
            UnityEngine.Debug.LogError($"Can't find clip with name: {animName}.");
            return -1;
        }

    }
}
