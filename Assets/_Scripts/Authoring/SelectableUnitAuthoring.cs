using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class SelectableUnitAuthoring : MonoBehaviour
    {

        private class Baker : Baker<SelectableUnitAuthoring>
        {
            public override void Bake(SelectableUnitAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<SelectableUnitTag>(entity);
                AddComponent(entity, new SelectedState
                {
                    Value = false,
                });

            }
        }
    }
}
