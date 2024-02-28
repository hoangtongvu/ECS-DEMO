using Components.CustomIdentification;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Camera
{
    public class CameraAuthoring : MonoBehaviour
    {

        private class Baker : Baker<CameraAuthoring>
        {
            public override void Bake(CameraAuthoring authoring)
            {
                Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

                AddComponent(entity, new UniqueId
                {
                    Kind = UniqueKind.Camera,
                    Id = 0,
                });

            }
        }
    }
}
