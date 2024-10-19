using Unity.Entities;
using Unity.Burst;
using Components.GameResource;
using Unity.Transforms;
using Components.Unit.NearUnitDropItems;
using Utilities;

namespace Systems.Simulation.Unit.NearUnitDropItems
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct TimeUpSetUnitDropItemsSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new NearbyUnitDropItemTimeLimit
                {
                    Value = 4f,
                });


            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    NearbyUnitDropItemTimerElement>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate<NearbyUnitDropItemTimeLimit>();

        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var spawnCommandList = SystemAPI.GetSingleton<ResourceItemSpawnCommandList>();
            float timeLimitSecond = SystemAPI.GetSingleton<NearbyUnitDropItemTimeLimit>().Value;

            foreach (var timerList in
                SystemAPI.Query<
                    DynamicBuffer<NearbyUnitDropItemTimerElement>>())
            {

                int length = timerList.Length;

                for (int i = 0; i < length; i++)
                {
                    ref var timer = ref timerList.ElementAt(i);
                    bool timeUp = timer.CounterSecond >= timeLimitSecond;

                    if (!timeUp) continue;

                    timer.CounterSecond = 0;

                    this.SetUnitDropItems(ref state, in timer.UnitEntity, in spawnCommandList);

                }

            }


        }


        [BurstCompile]
        private void SetUnitDropItems(
            ref SystemState state
            , in Entity unitEntity
            , in ResourceItemSpawnCommandList spawnCommandList)
        {
            var unitTransformRef = SystemAPI.GetComponentRO<LocalTransform>(unitEntity);
            var unitWallet = SystemAPI.GetBuffer<ResourceWalletElement>(unitEntity);
            int walletLength = unitWallet.Length;

            for (int j = 0; j < walletLength; j++)
            {
                ref var walletElement = ref unitWallet.ElementAt(j);

                if (walletElement.Quantity == 0) continue;

                spawnCommandList.Value.Add(new()
                {
                    SpawnPos = unitTransformRef.ValueRO.Position,
                    ResourceType = walletElement.Type,
                    Quantity = walletElement.Quantity,
                });

                walletElement.Quantity = 0;
            }

        }



    }

}