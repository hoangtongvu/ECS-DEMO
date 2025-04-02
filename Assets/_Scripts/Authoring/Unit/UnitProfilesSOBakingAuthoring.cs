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
                    buffer.Add(new()
                    {
                        OriginalPresenterGO = profile.Value.PresenterPrefab,
                        PrimaryEntity = GetEntity(profile.Value.PrimaryEntityPrefab, TransformUsageFlags.Dynamic),
                        PresenterEntity = profile.Value.IsPresenterEntity
                            ? GetEntity(profile.Value.PresenterPrefab, TransformUsageFlags.None)
                            : Entity.Null,
                        GameEntitySize = profile.Value.GameEntitySize,
                    });

                }
                
            }

        }

    }

}
