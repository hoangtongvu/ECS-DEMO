using Components.Misc.GameView;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.Misc.GameView
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct GameViewChangedTagClearSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameViewChangedTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var tagRef in SystemAPI.Query<RefRW<GameViewChangedTag>>())
            {
                tagRef.ValueRW.Value = false;
            }
        }

    }

}