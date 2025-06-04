using Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring.Misc
{
    public class UISpawnedAuthoring : MonoBehaviour
    {
        public float3 SpawnPosOffset = new(0, 3, 0);

        private class Baker : Baker<UISpawnedAuthoring>
        {
            public override void Bake(UISpawnedAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);


                AddComponent(entity, new UISpawned
                {
                    UIID = null,
                    SpawnPosOffset = authoring.SpawnPosOffset,
                    IsSpawned = false,
                });

            }

        }

    }

}
