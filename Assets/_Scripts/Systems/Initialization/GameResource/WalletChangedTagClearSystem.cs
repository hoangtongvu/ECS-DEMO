using Components.GameResource;
using Unity.Burst;
using Unity.Entities;
using Unity.Scenes;

namespace Systems.Initialization.GameResource
{

    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(SceneSystemGroup))]
    [BurstCompile]
    public partial struct WalletChangedTagClearSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WalletChangedTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var tagRef in SystemAPI.Query<EnabledRefRW<WalletChangedTag>>())
            {
                tagRef.ValueRW = false;
            }
        }

    }
}