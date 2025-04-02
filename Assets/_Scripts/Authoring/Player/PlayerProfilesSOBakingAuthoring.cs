using Authoring.Utilities.Helpers.GameEntity;
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

                ProfilesSOBakingHelper.BakeGameEntityProfileElementBuffer(this, authoring.profilesSO, entity);

            }

        }

    }

}
