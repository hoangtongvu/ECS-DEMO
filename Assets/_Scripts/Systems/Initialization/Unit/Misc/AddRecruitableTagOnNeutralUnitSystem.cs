using Components.GameEntity.EntitySpawning;
using Components.GameEntity.Misc;
using Components.Unit.Misc;
using Systems.Initialization.GameEntity.Misc;
using Systems.Initialization.Misc;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Initialization.Unit.Misc
{
    [UpdateInGroup(typeof(NewlySpawnedTagProcessSystemGroup))]
    [UpdateAfter(typeof(SetSpawnerFactionToSpawnedEntitySystem))]
    [BurstCompile]
    public partial struct AddRecruitableTagOnNeutralUnitSystem : ISystem
    {
        private EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitTag
                    , FactionIndex>()
                .WithAll<
                    NewlySpawnedTag>()
                .Build();

            state.RequireForUpdate(this.query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.query.ToEntityArray(Allocator.Temp);
            int length = entities.Length;

            if (length == 0) return;

            var em = state.EntityManager;
            var factionIndices = this.query.ToComponentDataArray<FactionIndex>(Allocator.Temp);

            for (int i = 0; i < length; i++)
            {
                var entity = entities[i];
                var factionIndex = factionIndices[i];

                if (factionIndex.Value != 0) continue;

                em.AddComponent<CanBeRecruitedTag>(entity);
            }

        }

    }

}