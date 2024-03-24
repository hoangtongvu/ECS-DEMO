using Components.Damage;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Damage
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

                AddComponent(entity, new HpComponent
                {
                    CurrentHp = authoring.CurrentHp,
                    MaxHp = authoring.MaxHp,
                });

                AddComponent(entity, new HpChangeState
                {
                    IsChanged = false,
                    ChangedValue = 0,
                });

                AddComponent(entity, new AliveState
                {
                    Value = true,
                });

                

            }
        }
    }
}
