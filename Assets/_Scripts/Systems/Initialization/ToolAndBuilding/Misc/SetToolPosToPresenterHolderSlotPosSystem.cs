using Components.GameEntity.EntitySpawning;
using Unity.Entities;
using Unity.Transforms;
using Systems.Initialization.GameEntity.EntitySpawning;
using Components.Tool;
using Components.Misc.Presenter;
using Core.ToolAndBuilding.ToolSpawnerBuilding.Presenter;
using Unity.Physics;

namespace Systems.Initialization.ToolAndBuilding.Misc
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(SpawnPrefabSystem))]
    public partial class SetToolPosToPresenterHolderSlotPosSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    NewlySpawnedTag
                    , DerelictToolTag
                    , LocalTransform
                    , SpawnerEntityHolder
                    , PhysicsGravityFactor>()
                .Build();

            this.RequireForUpdate(entityQuery);
        }

        protected override void OnUpdate()
        {
            foreach (var (spawnerEntityHolderRef, transformRef, physicsGravityFactorRef, entity) in SystemAPI
                .Query<
                    RefRO<SpawnerEntityHolder>
                    , RefRW<LocalTransform>
                    , RefRW<PhysicsGravityFactor>>()
                .WithAll<
                    NewlySpawnedTag
                    , DerelictToolTag>()
                .WithEntityAccess())
            {
                var spawnerEntity = spawnerEntityHolderRef.ValueRO.Value;
                if (spawnerEntity == Entity.Null) continue;

                var spawnedEntities = SystemAPI.GetComponent<SpawnedEntityArray>(spawnerEntity);
                var spawnerPresenterHolder = SystemAPI.GetComponent<PresenterHolder>(spawnerEntity);
                var toolSpawnerPresenter = (ToolSpawnerPresenter)spawnerPresenterHolder.Value.Value;
                var toolHolderSlots = toolSpawnerPresenter.ToolHolderSlotMarkers;

                int count = spawnedEntities.Value.Length;

                for (int i = 0; i < count; i++)
                {
                    if (spawnedEntities.Value[i] != entity) continue;

                    int slotIndex = i;
                    transformRef.ValueRW.Position = toolHolderSlots[slotIndex].transform.position;
                    break;
                }

                physicsGravityFactorRef.ValueRW.Value = 0f;
            }

        }

    }

}