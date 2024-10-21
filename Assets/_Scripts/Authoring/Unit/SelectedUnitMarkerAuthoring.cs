using Components.Unit;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Unit
{
    public class SelectedUnitMarkerAuthoring : MonoBehaviour
    {

        private class Baker : Baker<SelectedUnitMarkerAuthoring>
        {
            public override void Bake(SelectedUnitMarkerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<SelectedUnitMarkerTag>(entity);

            }
        }
    }
}
