using Unity.Entities;
using Unity.Burst;
using Components.GameResource;
using Unity.Transforms;
using Components.Unit.NearUnitDropItems;

namespace Systems.Simulation.Unit.NearUnitDropItems
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct TimeUpSetUnitDropItemsSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    NearbyUnitDropItemTimerElement>()
                .Build();

            state.RequireForUpdate(query0);
        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            const float timeLimitSecond = 4f;
            var spawnCommandList = SystemAPI.GetSingleton<ResourceItemSpawnCommandList>();

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

                    // Set unit drop item
                    // reset timer

                    timer.CounterSecond = 0;

                    var unitTransformRef = SystemAPI.GetComponentRO<LocalTransform>(timer.UnitEntity);
                    var unitWallet = SystemAPI.GetBuffer<ResourceWalletElement>(timer.UnitEntity);
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

                        UnityEngine.Debug.Log("Dropped");

                        walletElement.Quantity = 0;
                    }

                    

                }

            }
        }



        

    }

}