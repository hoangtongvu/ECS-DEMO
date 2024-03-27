using Components.Unit;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Unit
{
    public class SelectableUnitAuthoring : MonoBehaviour
    {

        private class Baker : Baker<SelectableUnitAuthoring>
        {
            public override void Bake(SelectableUnitAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<SelectableUnitTag>(entity);
            }
        }
    }
}
