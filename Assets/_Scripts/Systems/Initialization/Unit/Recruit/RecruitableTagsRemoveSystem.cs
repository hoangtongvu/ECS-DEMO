using Components.Unit.Recruit;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Initialization.Unit.Recruit
{
    [UpdateInGroup(typeof(RecruitableTagsHandleSystemGroup), OrderLast = true)]
    [BurstCompile]
    public partial struct RecruitableTagsRemoveSystem : ISystem
    {
        private EntityQuery entityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    NewlyRecruitedTag
                    , CanBeRecruitedTag>()
                .Build();

            state.RequireForUpdate(this.entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.entityQuery.ToEntityArray(Allocator.Temp);
            int length = entities.Length;

            if (length == 0) return;

            var em = state.EntityManager;

            for (int i = 0; i < length; i++)
            {
                var entity = entities[i];

                em.RemoveComponent<CanBeRecruitedTag>(entity);
                em.RemoveComponent<NewlyRecruitedTag>(entity);
            }

        }

    }

}