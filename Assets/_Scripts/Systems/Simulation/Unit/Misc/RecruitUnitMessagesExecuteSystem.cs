using Components.GameEntity;
using Components.GameEntity.EntitySpawning.SpawningProfiles;
using Components.GameEntity.EntitySpawning.SpawningProfiles.Containers;
using Components.GameEntity.InteractableActions;
using Components.GameEntity.Misc;
using Components.GameResource;
using Components.Player;
using Components.Unit.Misc;
using Systems.Simulation.Unit.InteractableActions;
using Unity.Burst;
using Unity.Entities;
using Utilities.Helpers;

namespace Systems.Simulation.Unit.Misc
{
    [UpdateInGroup(typeof(ActionsTriggeredSystemGroup))]
    [BurstCompile]
    public partial struct RecruitUnitMessagesExecuteSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<RecruitUnitMessageList>();
            state.RequireForUpdate<EntityToContainerIndexMap>();
            state.RequireForUpdate<EntitySpawningCostsContainer>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var recruitUnitMessageList = SystemAPI.GetSingleton<RecruitUnitMessageList>().Value;
            var entityToContainerIndexMap = SystemAPI.GetSingleton<EntityToContainerIndexMap>();
            var entitySpawningCostsContainer = SystemAPI.GetSingleton<EntitySpawningCostsContainer>();

            this.GetPlayerComponents(ref state, out var resourceWallet, out var walletChangedTag, out var playerFactionIndex);

            int count = recruitUnitMessageList.Length;

            for (int i = 0; i < count; i++)
            {
                Entity baseEntity = recruitUnitMessageList[i].BaseEntity;
                Entity primaryEntity = SystemAPI.GetComponent<PrimaryPrefabEntityHolder>(baseEntity);
                var unitFactionIndexRef = SystemAPI.GetComponentRW<FactionIndex>(baseEntity);

                bool isUnitHired = unitFactionIndexRef.ValueRO != FactionIndex.Neutral;
                if (isUnitHired) continue;

                bool canSpendResources = ResourceWalletHelper.TrySpendResources(
                    ref resourceWallet
                    , ref walletChangedTag
                    , in entityToContainerIndexMap
                    , in entitySpawningCostsContainer
                    , in primaryEntity);

                if (!canSpendResources) continue;

                unitFactionIndexRef.ValueRW = playerFactionIndex;
                SystemAPI.SetComponentEnabled<NewlyActionTriggeredTag>(baseEntity, true);

            }

        }

        [BurstCompile]
        private void GetPlayerComponents(
            ref SystemState state
            , out DynamicBuffer<ResourceWalletElement> resourceWallet
            , out EnabledRefRW<WalletChangedTag> walletChangedTag
            , out FactionIndex factionIndex)
        {
            resourceWallet = default;
            walletChangedTag = default;
            factionIndex = default;

            foreach (var item in
                SystemAPI.Query<
                    DynamicBuffer<ResourceWalletElement>
                    , EnabledRefRW<WalletChangedTag>
                    , RefRO<FactionIndex>>()
                    .WithAll<PlayerTag>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                resourceWallet = item.Item1;
                walletChangedTag = item.Item2;
                factionIndex = item.Item3.ValueRO;
            }

        }

    }

}