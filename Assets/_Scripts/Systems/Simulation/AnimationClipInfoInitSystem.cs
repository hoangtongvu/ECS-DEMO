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

        private BaseAnimatorMap baseAnimatorMap;

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

        protected override void OnStartRunning()
        {
            this.SetBaseAnimatorMap();
        }

        protected override void OnUpdate()
        {

            foreach (var (idRef, clipInfos) in
                SystemAPI.Query<
                    RefRO<UniqueIdICD>
                    , DynamicBuffer<AnimationClipInfoElement>>()
                    .WithAll<NewlySpawnedTag>())
            {

                // Get corresponding BaseAnimator from Map.
                if (!this.baseAnimatorMap.Value.TryGetValue(idRef.ValueRO, out Core.Animator.BaseAnimator baseAnimator))
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

        private void SetBaseAnimatorMap()
        {
            if (SystemAPI.ManagedAPI.TryGetSingleton(out this.baseAnimatorMap)) return;
            UnityEngine.Debug.LogError("BaseAnimatorMap Singleton not found");
        }

    }
}