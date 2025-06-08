using TweenLib.ShakeTween.Data;
using TweenLib.Timer.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Misc
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct TweenMapsInitSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var su = SingletonUtilities.GetInstance(state.EntityManager);

            su.AddOrSetComponentData(new TimerList
            {
                Value = new(100, Allocator.Persistent),
            });

            su.AddOrSetComponentData(new TimerIdPool
            {
                Value = new(Allocator.Persistent),
            });

            su.AddOrSetComponentData(new ShakeDataList
            {
                Value = new(100, Allocator.Persistent),
            });

            su.AddOrSetComponentData(new ShakeDataIdPool
            {
                Value = new(Allocator.Persistent),
            });

            state.Enabled = false;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
        }

    }

}