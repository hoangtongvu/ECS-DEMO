using Components.Misc.Presenter;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Initialization.Misc.Presenter
{
    [UpdateInGroup(typeof(NeedSpawnPresenterTagProcessSystemGroup), OrderLast = true)]
    [BurstCompile]
    public partial struct ClearNeedSpawnPresenterTagsSystem : ISystem
    {
        private EntityQuery entityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    NeedSpawnPresenterTag>()
                .Build();

            state.RequireForUpdate(this.entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.entityQuery.ToEntityArray(Allocator.Temp);
            state.EntityManager.RemoveComponent<NeedSpawnPresenterTag>(entities);

            entities.Dispose();

        }

    }

}