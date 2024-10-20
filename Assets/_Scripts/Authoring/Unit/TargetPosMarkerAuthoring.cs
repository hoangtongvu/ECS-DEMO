using Components.Unit;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Unit
{
    public class TargetPosMarkerAuthoring : MonoBehaviour
    {

        private class Baker : Baker<TargetPosMarkerAuthoring>
        {
            public override void Bake(TargetPosMarkerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<TargetPosMarkerTag>(entity);
            }
        }
    }
}
