using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class AnimatorAuthoring : MonoBehaviour
    {
        public string CurrentStateName = "Idle";
        

        private class Baker : Baker<AnimatorAuthoring>
        {
            public override void Bake(AnimatorAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);


                AddComponent(entity, new AnimatorData
                {
                    AnimName = authoring.CurrentStateName,
                    AnimChanged = false,
                });

                AddBuffer<AnimationClipInfoElement>(entity);

            }
        }
    }
}
