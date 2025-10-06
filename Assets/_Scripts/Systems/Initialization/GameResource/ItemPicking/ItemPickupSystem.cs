using Components.GameEntity.Interaction.InteractionPhases;
using Components.GameResource;
using Components.GameResource.ItemPicking;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Utilities.Helpers;

namespace Systems.Initialization.GameResource.ItemPicking
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct ItemPickupSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    CandidateItemDistanceHit
                    , ItemCanBePickedUpIndex
                    , ResourceWalletElement
                    , WalletChangedTag>()
                .WithAll<
                    InteractingPhase.Updating>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (interactingUpdatingTag, distanceHits, pickableItemIndexes, resourceWallet, walletChangedTag) in SystemAPI
                .Query<
                    EnabledRefRO<InteractingPhase.Updating>
                    , DynamicBuffer<CandidateItemDistanceHit>
                    , DynamicBuffer<ItemCanBePickedUpIndex>
                    , DynamicBuffer<ResourceWalletElement>
                    , EnabledRefRW<WalletChangedTag>>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                if (!interactingUpdatingTag.ValueRO) continue;

                foreach (var index in pickableItemIndexes)
                {
                    var itemEntity = distanceHits[index].Entity;
                    var resourceItemICD = SystemAPI.GetComponent<ResourceItemICD>(itemEntity);

                    bool canAddResourceToWallet = ResourceWalletHelper.TryAddResourceOfType(
                        in resourceWallet
                        , walletChangedTag
                        , resourceItemICD.ResourceType
                        , resourceItemICD.Quantity);

                    if (!canAddResourceToWallet) continue;

                    ecb.DestroyEntity(itemEntity);
                }

                pickableItemIndexes.Clear();
            }

            ecb.Playback(state.EntityManager);
        }

    }

}