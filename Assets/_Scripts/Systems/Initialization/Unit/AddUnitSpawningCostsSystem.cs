using Components;
using Components.GameEntity;
using Components.MyEntity.EntitySpawning.GlobalCostMap;
using Components.MyEntity.EntitySpawning.GlobalCostMap.Containers;
using Components.Unit;
using Core.GameResource;
using Unity.Entities;
using Unity.Scenes;

namespace Systems.Initialization.Unit
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(SceneSystemGroup))]
    public partial class AddUnitSpawningCostsSystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitProfilesSOHolder
                    , AfterBakedPrefabsElement>()
                .Build();

            this.RequireForUpdate(this.query);
            this.RequireForUpdate<EnumLength<ResourceType>>();

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
            var profilesSOHolder = this.query.GetSingleton<UnitProfilesSOHolder>();
            var afterBakedPrefabsBuffer = this.query.GetSingletonBuffer<AfterBakedPrefabsElement>();

            int resourceCount = SystemAPI.GetSingleton<EnumLength<ResourceType>>().Value;
            var latestCostMapIndexRef = SystemAPI.GetSingletonRW<LatestCostMapIndex>();
            var entityToCostMapIndexMap = SystemAPI.GetSingleton<EntityToCostMapIndexMap>();
            var entitySpawningCostsContainer = SystemAPI.GetSingleton<EntitySpawningCostsContainer>();
            var durationsContainer = SystemAPI.GetSingleton<EntitySpawningDurationsContainer>();
            var spritesContainer = SystemAPI.GetSingleton<EntitySpawningSpritesContainer>();

            int tempIndex = 0;

            foreach (var profile in profilesSOHolder.Value.Value.Profiles)
            {
                int nextMapIndex = latestCostMapIndexRef.ValueRO.Value + 1;

                var key = afterBakedPrefabsBuffer[tempIndex].PrimaryEntity;
                var value = nextMapIndex;

                if (!entityToCostMapIndexMap.Value.TryAdd(key, value)) continue;

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