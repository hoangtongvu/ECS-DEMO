using Unity.Entities;
using Unity.Burst;
using Components.GameResource;
using Unity.Collections;
using Unity.Transforms;
using Unity.Physics;
using Core;
using Components.Misc;
using Utilities.Helpers;
using Components.Misc.GlobalConfigs;

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
                    , WalletChangedTag>()
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

            var gameGlobalConfigs = SystemAPI.GetSingleton<GameGlobalConfigsICD>();
            float interactRadius = gameGlobalConfigs.Value.UnitInteractRadius;

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
                        , interactRadius
                        , ref hitList
                        , new CollisionFilter
                        {
                            BelongsTo = (uint) CollisionLayer.Default, // This make the hit able to collide with Player/Unit layer
                            CollidesWith = (uint) (CollisionLayer.Unit | CollisionLayer.Player),
                        });

                if (!hasHit) continue;


                //This can be made a job
                int length = hitList.Length;

                for (int i = 0; i < length; i++)
                {
                    var hitEntity = hitList[i].Entity;

                    if (!SystemAPI.HasComponent<ItemPickerTag>(hitEntity)) continue;
                    if (!SystemAPI.IsComponentEnabled<ItemPickerTag>(hitEntity)) continue;

                    var resourceWallet = SystemAPI.GetBuffer<ResourceWalletElement>(hitEntity);
                    var walletChangedTagLookup = SystemAPI.GetComponentLookup<WalletChangedTag>();

                    bool canAddResourcesToWallet = ResourceWalletHelper.TryAddResources(
                        in walletChangedTagLookup
                        , in hitEntity
                        , in resourceWallet
                        , resourceItemICDRef.ValueRO.ResourceType
                        , resourceItemICDRef.ValueRO.Quantity);

                    if (!canAddResourcesToWallet) continue;

                    ecb.DestroyEntity(itemEntity);
                    break;

                }
            }
        }


    }

}