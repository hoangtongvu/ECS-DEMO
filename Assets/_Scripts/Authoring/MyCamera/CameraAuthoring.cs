using Components.MyCamera;
using TweenLib.StandardTweeners;
using Unity.Entities;
using UnityEngine;
using Utilities.Tweeners.Camera;

namespace Authoring.MyCamera
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

                AddPosYTweener.AddTweenComponents(this, entity);
                AddPosXZTweener.AddTweenComponents(this, entity);
                TransformRotationTweener.AddTweenComponents(this, entity);

            }
        }
    }
}
