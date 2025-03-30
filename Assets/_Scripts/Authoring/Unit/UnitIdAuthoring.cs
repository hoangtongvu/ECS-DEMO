using Core.Unit;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Unit
{
    public class UnitIdAuthoring : MonoBehaviour
    {
        [SerializeField] private UnitType unitType;
        [SerializeField] private ushort localIndex;

        private class Baker : Baker<UnitIdAuthoring>
        {
            public override void Bake(UnitIdAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent(entity, new Components.Unit.UnitId
                {
                    UnitType = authoring.unitType,
                    LocalIndex = authoring.localIndex,
                });

            }

        }

    }

}
