using Components.GameEntity;
using Core.GameEntity;
using Unity.Entities;

namespace Authoring.Utilities.Helpers.GameEntity
{
    public static class ProfilesSOBakingHelper
    {
        public static void BakeGameEntityProfileElementBuffer<IdType, ProfileType>(
            IBaker baker
            , GameEntityProfilesSO<IdType, ProfileType> gameEntityProfilesSO
            , in Entity bakerEntity)
            where IdType : unmanaged
            where ProfileType : GameEntityProfileElement
        {
            var buffer = baker.AddBuffer<BakedGameEntityProfileElement>(bakerEntity);

            foreach (var profile in gameEntityProfilesSO.Profiles)
            {
                buffer.Add(new()
                {
                    OriginalPresenterGO = profile.Value.PresenterPrefab,
                    PrimaryEntity = baker.GetEntity(profile.Value.PrimaryEntityPrefab, TransformUsageFlags.Dynamic),
                    PresenterEntity = profile.Value.IsPresenterEntity
                        ? baker.GetEntity(profile.Value.PresenterPrefab, TransformUsageFlags.None)
                        : Entity.Null,
                    GameEntitySize = profile.Value.GameEntitySize,
                    HasHpComponents = profile.Value.HasHpComponents,
                    HpData = profile.Value.HpData,
                });

            }

        }

    }

}
