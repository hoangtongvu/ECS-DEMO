using Components.Unit;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Unit
{
    public class UnitToolHolderAuthoring : MonoBehaviour
    {

        private class Baker : Baker<UnitToolHolderAuthoring>
        {
            public override void Bake(UnitToolHolderAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new UnitToolHolder
                {
                    Value = Entity.Null,
                });
            }
        }
    }
}
