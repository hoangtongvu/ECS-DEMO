using Components.Player;
using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;

namespace Systems.Initialization.Player
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(AnimationClipInfoInitSystem))]
    [BurstCompile]
    public partial struct InitAttackDurationSystem : ISystem
    {
        private const string ATTACK_ANIM_NAME = "Punching";//TODO: Turn this into public Enum?

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<AttackData>();
            state.RequireForUpdate<AnimationClipInfoElement>();
            state.RequireForUpdate<PlayerTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            foreach (var (attackDataRef, clipInfos) in
                SystemAPI.Query<
                    RefRW<AttackData>
                    , DynamicBuffer<AnimationClipInfoElement>>()
                    .WithAll<PlayerTag>())
            {
                attackDataRef.ValueRW.attackDurationSecond = this.GetAttackDuration(clipInfos, ATTACK_ANIM_NAME);
            }
        }

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
