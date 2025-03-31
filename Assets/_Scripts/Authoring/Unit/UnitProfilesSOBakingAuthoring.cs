using Components.GameEntity;
using Components.Unit;
using Core.Unit;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Unit
{
    public class UnitProfilesSOBakingAuthoring : MonoBehaviour
    {
        [SerializeField] private UnitProfilesSO unitProfilesSO;

        private class Baker : Baker<UnitProfilesSOBakingAuthoring>
        {
            public override void Bake(UnitProfilesSOBakingAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new UnitProfilesSOHolder
                {
                    Value = authoring.unitProfilesSO,
                });

                var buffer = AddBuffer<AfterBakedPrefabsElement>(entity);

                foreach (var profile in authoring.unitProfilesSO.Profiles)
                {
                    var entityPrefabsElement = new AfterBakedPrefabsElement();

                    entityPrefabsElement.OriginalPresenterGO = profile.Value.PresenterPrefab;

                    if (profile.Value.IsPresenterEntity)
                    {
                        entityPrefabsElement.PresenterEntity = GetEntity(profile.Value.PresenterPrefab, TransformUsageFlags.None);
                    }

                    entityPrefabsElement.PrimaryEntity = GetEntity(profile.Value.PrimaryEntityPrefab, TransformUsageFlags.Dynamic);

                    buffer.Add(entityPrefabsElement);
                }
                
            }

        }

    }

}
