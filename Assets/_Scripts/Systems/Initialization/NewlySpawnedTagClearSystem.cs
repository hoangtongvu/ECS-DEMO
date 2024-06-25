using Components.Unit.UnitSpawning;
using Unity.Burst;
using Unity.Entities;
using Unity.Scenes;

namespace Systems.Initialization
{

    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(SceneSystemGroup))]
    [BurstCompile]
    public partial struct NewlySpawnedTagClearSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NewlySpawnedTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //new DisableTagJob()
            //    .ScheduleParallel();

            foreach (var tagRef in SystemAPI.Query<EnabledRefRW<NewlySpawnedTag>>())
            {
                tagRef.ValueRW = false;
            }
        }

    }
}