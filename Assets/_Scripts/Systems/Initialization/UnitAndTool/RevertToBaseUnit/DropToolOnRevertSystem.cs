using Components.GameEntity;
using Components.GameEntity.Misc;
using Components.Unit;
using Components.Unit.Misc;
using Components.Unit.RevertToBaseUnit;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Systems.Initialization.UnitAndTool.RevertToBaseUnit
{
    [UpdateInGroup(typeof(RevertToBaseUnitSystemGroup))]
    [UpdateBefore(typeof(ResetToolHolderSystem))]
    [BurstCompile]
    public partial struct DropToolOnRevertSystem : ISystem
    {
        private EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitTag
                    , UnitToolHolder
                    , NeedRevertToBaseUnitTag>()
                .Build();

            state.RequireForUpdate(this.query);
            state.RequireForUpdate<SetPosWithinRadiusCommandList>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var commandList = SystemAPI.GetSingleton<SetPosWithinRadiusCommandList>().Value;
            var unitEntities = this.query.ToEntityArray(Allocator.Temp);
            var unitToolHolders = this.query.ToComponentDataArray<UnitToolHolder>(Allocator.Temp);

            var em = state.EntityManager;
            int length = unitEntities.Length;

            for (int i = 0; i < length; i++)
            {
                Entity toolEntity = unitToolHolders[i].Value;
                Entity toolPrefabEntity = SystemAPI.GetComponent<PrimaryPrefabEntityHolder>(toolEntity);

                em.DestroyEntity(toolEntity);
                toolEntity = em.Instantiate(toolPrefabEntity);

                commandList.Add(new()
                {
                    BaseEntity = toolEntity,
                    CenterPos = SystemAPI.GetComponent<LocalTransform>(unitEntities[i]).Position,
                    Radius = 3f,
                });

            }

        }

    }

}
