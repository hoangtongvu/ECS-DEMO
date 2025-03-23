using Components.Misc.WorldMap;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Utilities;

namespace Systems.Initialization.Misc.WorldMap
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    [BurstCompile]
    public partial struct NeighborCellDirectionsInitSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            NativeArray<int2> orders = new(8, Allocator.Persistent);

            /*
             * 7 0 4
             * 3 x 1
             * 6 2 5
             */
            orders[0] = new(0, -1);
            orders[1] = new(1, 0);
            orders[2] = new(0, 1);
            orders[3] = new(-1, 0);
            orders[4] = new(1, -1);
            orders[5] = new(1, 1);
            orders[6] = new(-1, 1);
            orders[7] = new(-1, -1);

            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new NeighborCellDirections
                {
                    Value = orders,
                });

            state.Enabled = false;

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
        }

    }

}