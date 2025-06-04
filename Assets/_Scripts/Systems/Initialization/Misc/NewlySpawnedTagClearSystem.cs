using Components.GameEntity.EntitySpawning;
using Unity.Burst;
using Unity.Entities;
using Unity.Scenes;

namespace Systems.Initialization.Misc
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
            foreach (var tagRef in SystemAPI.Query<EnabledRefRW<NewlySpawnedTag>>())
            {
                tagRef.ValueRW = false;
            }

        }

    }

}