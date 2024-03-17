using Components;
using Components.CustomIdentification;
using Unity.Entities;

namespace Systems.Initialization
{

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(BaseAnimatorMapSystem))]
    public partial class AnimationClipInfoInitSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<AnimationClipInfoElement>();

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;


            // Get BaseAnimatorMap.
            if (!this.TryGetBaseAnimatorMap(out BaseAnimatorMap baseAnimatorMap)) return;


            // Run a Query of UniqueId then Get corresponding BaseAnimator from Map.
            foreach (var (idRef, clipInfos) in
                SystemAPI.Query<
                    RefRO<UniqueId>
                    , DynamicBuffer<AnimationClipInfoElement>>())
            {
                if (!baseAnimatorMap.Value.TryGetValue(idRef.ValueRO, out Core.Animator.BaseAnimator baseAnimator))
                {
                    UnityEngine.Debug.LogError($"Can't get BaseAnimator with {idRef.ValueRO} in BaseAnimatorMap.");
                    continue;
                }


                // Add all animation info from BaseAnimator to DynamicBuffer<AnimationClipInfoElement>.
                baseAnimator.ClipList
                    .ForEach(c => clipInfos
                    .Add(new AnimationClipInfoElement
                    {
                        Name = c.name,
                        Length = c.length,
                    }));

            }

        }

        private bool TryGetBaseAnimatorMap(out BaseAnimatorMap baseAnimatorMap)
        {
            if (SystemAPI.ManagedAPI.TryGetSingleton<BaseAnimatorMap>(out baseAnimatorMap)) return true;
            UnityEngine.Debug.LogError("Can't get BaseAnimatorMap Singleton.");
            return false;
        }

    }
}