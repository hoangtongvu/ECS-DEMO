using Components.CustomIdentification;
using Core.CustomIdentification;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class UniqueIdAuthoring : MonoBehaviour
    {
        public UniqueId UniqueId;

        private class Baker : Baker<UniqueIdAuthoring>
        {
            public override void Bake(UniqueIdAuthoring authoring)
            {
                Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);

                AddComponent(entity, new UniqueIdICD
                {
                    BaseId = authoring.UniqueId,
                });

            }
        }
    }
}
