using Components.Camera;
using Unity.Entities;
using UnityEngine;
using Unity.Transforms;

namespace Authoring.Camera
{
    public class CameraAuthoring : MonoBehaviour
    {
        public Vector3 AddPos;

        private class Baker : Baker<CameraAuthoring>
        {
            public override void Bake(CameraAuthoring authoring)
            {
                Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

                AddComponent<CameraEntityTag>(entity);

                AddComponent(entity, new AddPos
                {
                    Value = authoring.AddPos,
                });

                AddComponent<AddPosTweener_TweenData>(entity);
                AddComponent<Can_AddPosTweener_TweenTag>(entity);
                SetComponentEnabled<Can_AddPosTweener_TweenTag>(entity, false);

                AddComponent<Can_TransformRotationTweener_TweenTag>(entity);
                SetComponentEnabled<Can_TransformRotationTweener_TweenTag>(entity, false);
                AddComponent<TransformRotationTweener_TweenData>(entity);

            }
        }
    }
}
