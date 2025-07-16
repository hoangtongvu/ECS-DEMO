using Components.Misc.WorldMap;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Initialization.Misc.WorldMap
{
    [UpdateInGroup(typeof(MapComponentsProcessSystemGroup), OrderLast = true)]
    [BurstCompile]
    public partial struct WorldMapChangedTagClearSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WorldMapChangedTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var tagRef in SystemAPI.Query<RefRW<WorldMapChangedTag>>())
            {
                tagRef.ValueRW.Value = false;
            }

        }

    }

}