using Components.GameEntity;
using Components.Misc.WorldMap.WorldBuilding;
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
                    PlayerBuildingProfilesSOHolder
                    , BakedGameEntityProfileElement>()
                .Build();

            this.RequireForUpdate(this.query);

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
            var profilesSOHolder = this.query.GetSingleton<PlayerBuildingProfilesSOHolder>();
            var bakedProfileElementArray = this.query.GetSingletonBuffer<BakedGameEntityProfileElement>().ToNativeArray(Allocator.Temp);
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            su.AddOrSetComponentData(new BuildableObjectChoiceIndex
            {
                Value = BuildableObjectChoiceIndex.NoChoice,
            });

            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var playerBuildableObjectElements = ecb.AddBuffer<PlayerBuildableObjectElement>(su.DefaultSingletonEntity);
            int tempIndex = 0;

            foreach (var profile in profilesSOHolder.Value.Value.Profiles)
            {
                playerBuildableObjectElements.Add(new()
                {
                    Entity = bakedProfileElementArray[tempIndex].PrimaryEntity,
                    PreviewSprite = profile.Value.ProfilePicture,
                    Name = profile.Value.Name,
                    GridSquareSize = profile.Value.GameEntitySize.GridSquareSize,
                    ObjectHeight = profile.Value.GameEntitySize.ObjectHeight,
                });

                tempIndex++;

            }

            bakedProfileElementArray.Dispose();
            ecb.Playback(this.EntityManager);
            ecb.Dispose();

        }

    }

}