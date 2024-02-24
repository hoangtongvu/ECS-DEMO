using Components.Player;
using UnityEngine;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Systems.Simulation.Player
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct PlayerRefsTransformSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerRefsTransform>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            //state.Enabled = false;
            var playerRefsTransformRef = SystemAPI.GetSingletonRW<PlayerRefsTransform>();
            foreach (var transformRef in SystemAPI.Query<RefRO<LocalTransform>>().WithAll<PlayerTag>())
            {
                playerRefsTransformRef.ValueRW.transform = transformRef.ValueRO;
            }
            
        }

    }
}
