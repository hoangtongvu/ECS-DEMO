using Components;
using Components.ComponentMap;
using Components.CustomIdentification;
using Components.MyEntity.EntitySpawning;
using Unity.Entities;

namespace Systems.Simulation
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class AnimationClipInfoInitSystem : SystemBase
    {
        protected override void OnCreate()
        {
            EntityQuery entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    UniqueIdICD
                    , AnimationClipInfoElement
                    , NewlySpawnedTag>()
                .Build();
            this.RequireForUpdate(entityQuery);
        }

        protected override void OnUpdate()
        {

            foreach (var (idRef, clipInfos) in
                SystemAPI.Query<
                    RefRO<UniqueIdICD>
                    , DynamicBuffer<AnimationClipInfoElement>>()
                    .WithAll<NewlySpawnedTag>())
            {
                // Get BaseAnimatorMap.
                if (!this.TryGetBaseAnimatorMap(out BaseAnimatorMap baseAnimatorMap)) return;

                // Get corresponding BaseAnimator from Map.
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