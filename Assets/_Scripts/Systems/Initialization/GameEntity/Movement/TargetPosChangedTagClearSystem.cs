using Components.GameEntity.Movement;
using Unity.Burst;
using Unity.Entities;
using Unity.Scenes;

namespace Systems.Initialization.GameEntity.Movement
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(SceneSystemGroup))]
    [BurstCompile]
    public partial struct TargetPosChangedTagClearSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TargetPosChangedTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var tagRef in SystemAPI.Query<EnabledRefRW<TargetPosChangedTag>>())
            {
                tagRef.ValueRW = false;
            }

        }

    }

}