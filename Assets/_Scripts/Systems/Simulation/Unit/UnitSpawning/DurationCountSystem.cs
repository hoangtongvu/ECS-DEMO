using Unity.Entities;
using Unity.Burst;
using Components.Unit.UnitSpawning;

namespace Systems.Simulation.Unit.UnitSpawning
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct DurationCountSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpawnCount>();
            state.RequireForUpdate<SpawnDuration>();
            state.RequireForUpdate<CanSpawnState>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Still create Job even when nothing to Spawn.
            var job = new CountUpJob()
            {
                DeltaTime = SystemAPI.Time.DeltaTime,
            };

            job.ScheduleParallel();
        }


        [BurstCompile]
        private partial struct CountUpJob : IJobEntity
        {
            public float DeltaTime;

            void Execute(
                ref SpawnCount spawnCount
                , ref SpawnDuration spawnDuration
                , ref CanSpawnState canSpawnState)
            {
                if (spawnCount.Value <= 0) return;
                
                spawnDuration.DurationCounterSecond += this.DeltaTime;

                if (spawnDuration.DurationCounterSecond >= spawnDuration.DurationPerSpawn)
                {
                    spawnDuration.DurationCounterSecond = 0;
                    canSpawnState.Value = true;
                    spawnCount.Value--;
                }
            }
        }
        


    }
}