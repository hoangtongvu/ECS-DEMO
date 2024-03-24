using Components.Damage;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Damage
{
    public class DmgSenderAuthoring : MonoBehaviour
    {
        public int Damage = 10;

        private class Baker : Baker<DmgSenderAuthoring>
        {
            public override void Bake(DmgSenderAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new DmgValue
                {
                    Value = authoring.Damage,
                });

            }
        }
    }
}
