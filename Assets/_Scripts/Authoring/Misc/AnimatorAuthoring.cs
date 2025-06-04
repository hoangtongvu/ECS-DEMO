using Components.Misc;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Misc
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
                    Value = new()
                    {
                        Value = authoring.CurrentStateName,
                        ValueChanged = false,
                    }
                });
                AddComponent<AnimatorTransitionDuration>(entity);

            }

        }

    }

}
