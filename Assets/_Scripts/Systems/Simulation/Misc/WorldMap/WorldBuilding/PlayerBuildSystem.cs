using Components.GameEntity.EntitySpawning.SpawningProfiles.Containers;
using Components.GameEntity.EntitySpawning.SpawningProfiles;
using Components.Misc.WorldMap.WorldBuilding;
using Components.Misc.WorldMap.WorldBuilding.PlacementPreview;
using Core.Utilities.Extensions;
using Systems.Simulation.Misc.WorldMap.WorldBuilding.PlacementPreview;
using Unity.Entities;
using UnityEngine;
using Components;
using Core.GameResource;
using Components.GameResource;
using Unity.Burst;
using Components.Player;
using Utilities.Helpers;

namespace Systems.Simulation.Misc.WorldMap.WorldBuilding
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(PlacementPreviewDataUpdateSystem))]
    public partial class PlayerBuildSystem : SystemBase
    {
        protected override void OnCreate()
        {
            this.RequireForUpdate<PlacementPreviewData>();
            this.RequireForUpdate<BuildableObjectChoiceIndex>();
            this.RequireForUpdate<PlayerBuildableObjectElement>();
            this.RequireForUpdate<EntityToContainerIndexMap>();
            this.RequireForUpdate<EntitySpawningCostsContainer>();
            this.RequireForUpdate<EnumLength<ResourceType>>();
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
            var resourceCount = SystemAPI.GetSingleton<EnumLength<ResourceType>>().Value;

            var prefabEntity = buildableObjectElement.Entity;

            this.GetWallet(out var resourceWallet, out var walletChangedTag);
            bool canSpendResources = ResourceWalletHelper.TrySpendResources(
                ref resourceWallet
                , ref walletChangedTag
                , in entityToContainerIndexMap
                , in entitySpawningCostsContainer
                , in prefabEntity
                , in resourceCount);

            if (!canSpendResources) return;

            commandQueue.Value.Add(new()
            {
                Entity = prefabEntity,
                TopLeftCellGridPos = placementPreviewData.TopLeftCellGridPos,
                BuildingCenterPos = placementPreviewData.BuildingCenterPosOnGround.Add(y: buildableObjectElement.ObjectHeight),
                GridSquareSize = buildableObjectElement.GridSquareSize,
            });

        }

        [BurstCompile]
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
