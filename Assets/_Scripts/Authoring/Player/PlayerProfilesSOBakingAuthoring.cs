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
