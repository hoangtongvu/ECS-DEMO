using Components;
using Components.GameEntity;
using Components.GameEntity.EntitySpawning.SpawningProfiles;
using Components.GameEntity.EntitySpawning.SpawningProfiles.Containers;
using Components.Tool;
using Core.GameResource;
using Systems.Initialization.GameEntity.EntitySpawning.SpawningProfiles;
using Unity.Entities;

namespace Systems.Initialization.Tool
{
    [UpdateInGroup(typeof(AddSpawningProfilesSystemGroup))]
    public partial class AddToolSpawningProfilesSystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    ToolProfilesSOHolder
                    , BakedGameEntityProfileElement>()
                .Build();

            this.RequireForUpdate(this.query);
            this.RequireForUpdate<EnumLength<ResourceType>>();

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
            var profilesSOHolder = this.query.GetSingleton<ToolProfilesSOHolder>();
            var bakedProfileElements = this.query.GetSingletonBuffer<BakedGameEntityProfileElement>();

            int resourceCount = SystemAPI.GetSingleton<EnumLength<ResourceType>>().Value;
            var latestCostMapIndexRef = SystemAPI.GetSingletonRW<LatestCostMapIndex>();
            var entityToContainerIndexMap = SystemAPI.GetSingleton<EntityToContainerIndexMap>();
            var entitySpawningCostsContainer = SystemAPI.GetSingleton<EntitySpawningCostsContainer>();
            var durationsContainer = SystemAPI.GetSingleton<EntitySpawningDurationsContainer>();
            var spritesContainer = SystemAPI.GetSingleton<EntitySpawningSpritesContainer>();

            int tempIndex = 0;

            foreach (var profile in profilesSOHolder.Value.Value.Profiles)
            {
                int nextMapIndex = latestCostMapIndexRef.ValueRO.Value + 1;

                var key = bakedProfileElements[tempIndex].PrimaryEntity;
                var value = nextMapIndex;

                if (!entityToContainerIndexMap.Value.TryAdd(key, value)) continue;

                durationsContainer.Value.Add(profile.Value.SpawnDurationSeconds);
                spritesContainer.Value.Add(profile.Value.ProfilePicture);

                for (int i = 0; i < resourceCount; i++)
                {
                    profile.Value.BaseSpawningCosts.TryGetValue((ResourceType)i, out uint tempCost);
                    entitySpawningCostsContainer.Value.Add(tempCost);
                }

                latestCostMapIndexRef.ValueRW.Value++;
                tempIndex++;

            }

        }

    }

}