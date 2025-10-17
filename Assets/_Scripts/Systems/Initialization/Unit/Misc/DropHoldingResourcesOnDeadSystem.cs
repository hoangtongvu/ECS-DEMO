using Components.GameEntity.Damage;
using Components.GameResource;
using Components.Unit;
using Systems.Initialization.GameEntity.Damage.DeadResolve;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Systems.Initialization.Unit.Misc
{
    [UpdateInGroup(typeof(DeadResolveSystemGroup))]
    [BurstCompile]
    public partial struct DropHoldingResourcesOnDeadSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    JoblessUnitTag
                    , LocalTransform
                    , ResourceWalletElement>()
                .WithAll<
                    DeadEvent>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate<ResourceItemSpawnCommandList>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var itemSpawnCommands = SystemAPI.GetSingleton<ResourceItemSpawnCommandList>().Value;

            foreach (var (transformRef, wallet) in SystemAPI
                .Query<
                    RefRO<LocalTransform>
                    , DynamicBuffer<ResourceWalletElement>>()
                .WithAll<JoblessUnitTag>()
                .WithAll<DeadEvent>())
            {
                foreach (var walletElement in wallet)
                {
                    itemSpawnCommands.Add(new()
                    {
                        SpawnerEntity = Entity.Null,
                        SpawnPos = transformRef.ValueRO.Position,
                        ResourceType = walletElement.Type,
                        Quantity = walletElement.Quantity,
                    });
                }
            }

        }

    }

}