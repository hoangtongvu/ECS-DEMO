using Components.GameEntity.EntitySpawning.SpawningProfiles.Containers;
using Components.GameEntity.EntitySpawning.SpawningProfiles;
using Components.Misc.WorldMap.WorldBuilding;
using Components.Misc.WorldMap.WorldBuilding.PlacementPreview;
using Core.Utilities.Extensions;
using Systems.Simulation.Misc.WorldMap.WorldBuilding.PlacementPreview;
using Unity.Entities;
using UnityEngine;
using Components.GameResource;
using Components.Player;
using Utilities.Helpers;

namespace Systems.Simulation.Misc.WorldMap.WorldBuilding
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PlacementPreviewDataUpdateSystem))]
    public partial class PlayerBuildSystem : SystemBase
    {
        private EntityQuery playerQuery;

        protected override void OnCreate()
        {
            this.playerQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerTag>()
                .Build();

            this.RequireForUpdate(this.playerQuery);
            this.RequireForUpdate<PlacementPreviewData>();
            this.RequireForUpdate<BuildableObjectChoiceIndex>();
            this.RequireForUpdate<PlayerBuildableObjectElement>();
            this.RequireForUpdate<EntityToContainerIndexMap>();
            this.RequireForUpdate<EntitySpawningCostsContainer>();
        }

        protected override void OnUpdate()
        {
            int choiceIndex = SystemAPI.GetSingleton<BuildableObjectChoiceIndex>().Value;

            if (choiceIndex == BuildableObjectChoiceIndex.NoChoice) return;
            if (!Input.GetMouseButtonDown(0)) return;

            var placementPreviewData = SystemAPI.GetSingleton<PlacementPreviewData>();
            if (!placementPreviewData.CanPlacementPreview) return;
            if (!placementPreviewData.IsBuildable) return;

            var commandQueue = SystemAPI.GetSingleton<BuildCommandQueue>();
            var buildableObjectElement = SystemAPI.GetSingletonBuffer<PlayerBuildableObjectElement>()[choiceIndex];
            var entityToContainerIndexMap = SystemAPI.GetSingleton<EntityToContainerIndexMap>();
            var entitySpawningCostsContainer = SystemAPI.GetSingleton<EntitySpawningCostsContainer>();

            var prefabEntity = buildableObjectElement.Entity;

            this.GetWallet(out var resourceWallet, out var walletChangedTag);
            bool canSpendResources = ResourceWalletHelper.TrySpendResources(
                ref resourceWallet
                , ref walletChangedTag
                , in entityToContainerIndexMap
                , in entitySpawningCostsContainer
                , in prefabEntity);

            if (!canSpendResources) return;

            var playerEntity = this.playerQuery.GetSingletonEntity();

            commandQueue.Value.Add(new()
            {
                Entity = prefabEntity,
                TopLeftCellGridPos = placementPreviewData.TopLeftCellGridPos,
                GameEntitySize = buildableObjectElement.GameEntitySize,
                SpawnerEntity = playerEntity,
            });

        }

        private void GetWallet(
            out DynamicBuffer<ResourceWalletElement> resourceWallet
            , out EnabledRefRW<WalletChangedTag> walletChangedTag)
        {
            resourceWallet = default;
            walletChangedTag = default;

            foreach (var item in
                SystemAPI.Query<
                    DynamicBuffer<ResourceWalletElement>
                    , EnabledRefRW<WalletChangedTag>>()
                    .WithAll<PlayerTag>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                resourceWallet = item.Item1;
                walletChangedTag = item.Item2;
            }

        }

    }

}
