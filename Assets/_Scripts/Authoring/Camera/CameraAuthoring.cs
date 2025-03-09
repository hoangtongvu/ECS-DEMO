using Components.Camera;
using Unity.Entities;
using UnityEngine;

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

            }
        }
    }
}
