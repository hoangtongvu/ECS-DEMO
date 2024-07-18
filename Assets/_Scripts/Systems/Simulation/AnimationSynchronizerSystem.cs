using Components;
using Components.ComponentMap;
using Components.CustomIdentification;
using Core.Animator;
using Unity.Entities;
using UnityEngine;

namespace Systems.Simulation
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class AnimationSynchronizerSystem : SystemBase
    {

        private BaseAnimatorMap baseAnimatorMap;


        protected override void OnCreate()
        {
            RequireForUpdate<UniqueIdICD>();
            RequireForUpdate<AnimatorData>();
        }

        protected override void OnUpdate()
        {

            if (this.baseAnimatorMap == null) this.SetBaseAnimatorMap();

            //Sync Transform from Entity world to corresponding Transform in GameObject world.
            this.SyncAnimation();

        }

        private void SetBaseAnimatorMap()
        {
            if (SystemAPI.ManagedAPI.TryGetSingleton<BaseAnimatorMap>(out this.baseAnimatorMap)) return;
            Debug.LogError("BaseAnimatorMap Singleton not found");
        }


        private void SyncAnimation()
        {
            foreach (var (idRef, animDataRef) in
                SystemAPI.Query<
                    RefRO<UniqueIdICD>
                    , RefRW<AnimatorData>>())
            {
                // Only sync Animation if AnimChanged Tag is true.
                if (!animDataRef.ValueRO.Value.ValueChanged) continue;

                if (!this.baseAnimatorMap.Value.TryGetValue(idRef.ValueRO, out BaseAnimator baseAnimator))
                {
                    Debug.LogError($"Can't get BaseAnimator with {idRef.ValueRO} in BaseAnimatorMap");
                    continue;
                }

                baseAnimator.ChangeAnimationState(animDataRef.ValueRO.Value.Value.ToString());
                animDataRef.ValueRW.Value.ValueChanged = false;

            }
        }

        

    }
}