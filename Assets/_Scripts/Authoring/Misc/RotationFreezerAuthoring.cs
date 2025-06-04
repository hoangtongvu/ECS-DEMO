using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Misc
{
    public class RotationFreezerAuthoring : MonoBehaviour
    {
        [SerializeField] private float x;
        [SerializeField] private float z;

        private class Baker : Baker<RotationFreezerAuthoring>
        {
            public override void Bake(RotationFreezerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new RotationFreezer
                {
                    X = authoring.x,
                    Z = authoring.z,
                });

            }

        }

    }

}
