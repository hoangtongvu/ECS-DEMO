using Unity.Entities;
using Unity.Burst;
using Components.GameEntity.EntitySpawning;
using Components.GameEntity.EntitySpawning.SpawningProcess;

namespace Systems.Simulation.GameEntity.EntitySpawning.SpawningProcess
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct SpawningProcessTagsSetSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    EntitySpawningProfileElement
                    , IsInSpawningProcessTag
                    , JustBeginSpawningProcessTag
                    , JustEndSpawningProcessTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new SetTagsJob()
                .ScheduleParallel(state.Dependency);

        }

        [WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)]
        [BurstCompile]
        private partial struct SetTagsJob : IJobEntity
        {
            [BurstCompile]
            void Execute(
                in DynamicBuffer<EntitySpawningProfileElement> profileElements
                , EnabledRefRW<IsInSpawningProcessTag> isInSpawningProcessTag
                , EnabledRefRW<JustBeginSpawningProcessTag> justBeginSpawningProcessTag
                , EnabledRefRW<JustEndSpawningProcessTag> justEndSpawningProcessTag)
            {
                bool isInSpawningProcess = false;

                for (int i = 0; i < profileElements.Length; i++)
                {
                    var profile = profileElements[i];
                    if (profile.SpawnCount.Value <= 0) continue;

                    isInSpawningProcess = true;
                    break;
                }

                justBeginSpawningProcessTag.ValueRW = false;
                justEndSpawningProcessTag.ValueRW = false;

                if (!isInSpawningProcessTag.ValueRO && isInSpawningProcess)
                    justBeginSpawningProcessTag.ValueRW = true;

                if (isInSpawningProcessTag.ValueRO && !isInSpawningProcess)
                    justEndSpawningProcessTag.ValueRW = true;

                isInSpawningProcessTag.ValueRW = isInSpawningProcess;
            }

        }

    }

}