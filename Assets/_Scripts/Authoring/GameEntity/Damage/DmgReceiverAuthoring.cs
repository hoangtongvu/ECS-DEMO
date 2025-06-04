using Components.GameEntity.Damage;
using Unity.Entities;
using UnityEngine;

namespace Authoring.GameEntity.Damage
{
    public class DmgReceiverAuthoring : MonoBehaviour
    {
        public int MaxHp = 100;
        public int CurrentHp = 100; // Can't set CurrentHp = MaxHp directly.

        private class Baker : Baker<DmgReceiverAuthoring>
        {
            public override void Bake(DmgReceiverAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new CurrentHp
                {
                    Value = authoring.CurrentHp,
                });

                AddComponent(entity, new MaxHp
                {
                    Value = authoring.MaxHp,
                });

                AddBuffer<HpChangeRecordElement>(entity);
                AddComponent<IsAliveTag>(entity);

            }

        }

    }

}
