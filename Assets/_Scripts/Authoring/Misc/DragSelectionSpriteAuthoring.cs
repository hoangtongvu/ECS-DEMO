using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Authoring.Misc
{
    public class DragSelectionSpriteAuthoring : MonoBehaviour
    {
        private class Baker : Baker<DragSelectionSpriteAuthoring>
        {
            public override void Bake(DragSelectionSpriteAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.NonUniformScale);

                AddComponent<DragSelectionSpriteTag>(entity);

                AddComponent(entity, new PostTransformMatrix
                {
                    Value = float4x4.TRS(
                        float3.zero
                        , quaternion.identity
                        , new(1)),
                });

            }

        }

    }

}