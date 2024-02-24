using Components.Player;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring.Player
{
    public class PlayerRefsTransformAuthoring : MonoBehaviour
    {
        

        private class Baker : Baker<PlayerRefsTransformAuthoring>
        {
            public override void Bake(PlayerRefsTransformAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<PlayerRefsTransform>(entity);
            }
        }
    }
}
