using Unity.Entities;
using Unity.Burst;
using Components.GameResource;
using Core.GameResource;
using Unity.Collections;
using Unity.Transforms;
using Unity.Physics;
using Core;
using Components.Misc;

namespace Systems.Simulation.GameResource
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct ResourceItemPickupSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    ResourceItemICD
                    , LocalTransform>()
                .Build();

            var query1 = SystemAPI.QueryBuilder()
                .WithAll<
                    ItemPickerTag
                    , ResourceWalletElement
                    , WalletChanged>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate(query1);
        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (resourceItemICDRef, transformRef, itemEntity) in
                SystemAPI.Query<
                    RefRO<ResourceItemICD>
                    , RefRW<LocalTransform>>()
                    .WithEntityAccess())
            {
                var hitList = new NativeList<DistanceHit>(Allocator.Temp);

                bool hasHit =
                    physicsWorld.OverlapSphere(
                        transformRef.ValueRO.Position
                        , 2f
                        , ref hitList
                        , new CollisionFilter
                        {
                            BelongsTo = (uint) CollisionLayer.Default, // This make the hit able to collide with Player/Unit layer
                            CollidesWith = (uint) (CollisionLayer.Unit | CollisionLayer.Player),
                        });

                if (!hasHit) continue;

                int length = hitList.Length;

                for (int i = 0; i < length; i++)
                {
                    var hitEntity = hitList[i].Entity;

                    if (!SystemAPI.HasComponent<ItemPickerTag>(hitEntity)) continue;
                    if (!SystemAPI.IsComponentEnabled<ItemPickerTag>(hitEntity)) continue;

                    var resourceWallet = SystemAPI.GetBuffer<ResourceWalletElement>(hitEntity);
                    var walletChangedRef = SystemAPI.GetComponentRW<WalletChanged>(hitEntity);

                    bool canAddResourcesToWallet = this.TryAddResourcesToWallet(
                        resourceWallet
                        , ref walletChangedRef.ValueRW
                        , resourceItemICDRef.ValueRO.ResourceType
                        , resourceItemICDRef.ValueRO.Quantity);

                    if (!canAddResourcesToWallet) continue;

                    ecb.DestroyEntity(itemEntity);
                    break;

                }
            }
        }

        [BurstCompile]
        private bool TryAddResourcesToWallet(
            DynamicBuffer<ResourceWalletElement> resourceWallet
            , ref WalletChanged walletChanged
            , ResourceType addType
            , uint addQuantity)
        {
            int walletLength = resourceWallet.Length;

            for (int i = 0; i < walletLength; i++)
            {
                ref var walletElement = ref resourceWallet.ElementAt(i);

                bool matchType = walletElement.Type == addType;
                if (!matchType) continue;

                walletElement.Quantity += addQuantity;
                walletChanged.Value = true;

                return true;
            }

            return false;
        }


    }

}