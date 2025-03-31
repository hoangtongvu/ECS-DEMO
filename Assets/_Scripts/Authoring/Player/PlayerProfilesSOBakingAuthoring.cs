using Components.GameEntity;
using Components.Player;
using Core.Player;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Player
{
    public class PlayerProfilesSOBakingAuthoring : MonoBehaviour
    {
        [SerializeField] private PlayerProfilesSO profilesSO;

        private class Baker : Baker<PlayerProfilesSOBakingAuthoring>
        {
            public override void Bake(PlayerProfilesSOBakingAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);

                AddComponent(entity, new PlayerProfilesSOHolder
                {
                    Value = authoring.profilesSO,
                });

                var buffer = AddBuffer<AfterBakedPrefabsElement>(entity);

                foreach (var profile in authoring.profilesSO.Profiles)
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
