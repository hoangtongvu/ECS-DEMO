using Components.GameEntity;
using Components.Misc.WorldMap.WorldBuilding;
using Components.Player;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Misc.WorldMap.WorldBuilding
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class AddPlayerBuildingElementsSystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    GameBuildingProfilesSOHolder
                    , BakedGameEntityProfileElement>()
                .Build();

            this.RequireForUpdate(this.query);
            this.RequireForUpdate<GameBuildingPrefabEntityMap>();
            this.RequireForUpdate<PlayerProfilesSOHolder>();

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
            var profilesSOHolder = SystemAPI.GetSingleton<PlayerProfilesSOHolder>();
            var gameBuildingProfiles = SystemAPI.GetSingleton<GameBuildingProfilesSOHolder>().Value.Value.Profiles;
            var gameBuildingPrefabEntityMap = SystemAPI.GetSingleton<GameBuildingPrefabEntityMap>().Value;

            var su = SingletonUtilities.GetInstance(this.EntityManager);

            su.AddOrSetComponentData(new BuildableObjectChoiceIndex
            {
                Value = BuildableObjectChoiceIndex.NoChoice,
            });

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var playerBuildableObjectElements = ecb.AddBuffer<PlayerBuildableObjectElement>(su.DefaultSingletonEntity);

            var firstPlayerProfile = profilesSOHolder.Value.Value.Profiles[0];

            foreach (var id in firstPlayerProfile.PlayerBuildingIds)
            {
                if (!gameBuildingProfiles.TryGetValue(id.Key, out var buildingProfile))
                    throw new KeyNotFoundException($"GameBuildingProfiles does not contain key: {id.Key}");

                if (!gameBuildingPrefabEntityMap.TryGetValue(id.Key, out var buildingPrefabEntity))
                    throw new KeyNotFoundException($"{nameof(GameBuildingPrefabEntityMap)} does not contain key: {id.Key}");

                playerBuildableObjectElements.Add(new()
                {
                    Entity = buildingPrefabEntity,
                    PreviewSprite = buildingProfile.ProfilePicture,
                    Name = buildingProfile.Name,
                    GridSquareSize = buildingProfile.GameEntitySize.GridSquareSize,
                    ObjectHeight = buildingProfile.GameEntitySize.ObjectHeight,
                });

            }

            ecb.Playback(this.EntityManager);
            ecb.Dispose();

        }

    }

}