using Components.GameEntity.EntitySpawning;
using Components.Tool;
using Components.Tool.Misc;
using Components.Unit.Misc;
using Components.UnitAndTool.Misc;
using Core.Tool;
using Systems.Initialization.Misc;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Initialization.UnitAndTool.Misc
{
    [UpdateInGroup(typeof(NewlySpawnedTagProcessSystemGroup))]
    [BurstCompile]
    public partial struct CreateAndAssignToolToUnitSystem : ISystem
    {
        private EntityQuery targetUnitQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.targetUnitQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitTag
                    , NewlySpawnedTag
                    , NeedCreateAndAssignTool>()
                .Build();

            state.RequireForUpdate(this.targetUnitQuery);
            state.RequireForUpdate<ToolProfileId2PrimaryEntityMap>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var unitEntities = this.targetUnitQuery.ToEntityArray(Allocator.Temp);
            int length = unitEntities.Length;

            if (length == 0) return;

            var em = state.EntityManager;
            var needCreateAndAssignTools = this.targetUnitQuery.ToComponentDataArray<NeedCreateAndAssignTool>(Allocator.Temp);
            var toolProfileId2PrimaryEntityMap = SystemAPI.GetSingleton<ToolProfileId2PrimaryEntityMap>().Value;

            for (int i = 0; i < length; i++)
            {
                var unitEntity = unitEntities[i];
                var toolId = needCreateAndAssignTools[i].Value;

                this.SpawnTool(in em, in toolProfileId2PrimaryEntityMap, in toolId, out var newToolEntity);
                this.MarkToolCanBePicked(ref state, in newToolEntity, in unitEntity);
                em.RemoveComponent<NeedCreateAndAssignTool>(unitEntity);
            }

        }

        [BurstCompile]
        private void SpawnTool(
            in EntityManager em
            , in NativeHashMap<ToolProfileId, Entity> toolProfileId2PrimaryEntityMap
            , in ToolProfileId prefabId
            , out Entity newToolEntity)
        {
            Entity prefabToSpawn = toolProfileId2PrimaryEntityMap[prefabId];
            newToolEntity = em.Instantiate(prefabToSpawn);
        }

        [BurstCompile]
        private void MarkToolCanBePicked(ref SystemState state, in Entity toolEntity, in Entity unitEntity)
        {
            SystemAPI.SetComponentEnabled<CanBePickedTag>(toolEntity, true);

            var toolPickerEntityRef = SystemAPI.GetComponentRW<ToolPickerEntity>(toolEntity);
            toolPickerEntityRef.ValueRW.Value = unitEntity;
        }

    }

}