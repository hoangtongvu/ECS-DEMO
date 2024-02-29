using Components.Camera;
using Components.CustomIdentification;
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

                AddComponent(entity, new AddPos
                {
                    Value = authoring.AddPos.ToFloat3(),
                });

            }
        }
    }
}
