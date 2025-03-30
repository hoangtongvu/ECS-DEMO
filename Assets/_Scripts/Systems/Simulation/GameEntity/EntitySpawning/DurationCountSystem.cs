using Unity.Entities;
using Unity.Burst;
using Components.GameEntity.EntitySpawning;

namespace Systems.Simulation.GameEntity.EntitySpawning
{

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct DurationCountSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EntitySpawningProfileElement>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
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
                ref DynamicBuffer<EntitySpawningProfileElement> profileElements)
            {
                
                for (int i = 0; i < profileElements.Length; i++)
                {
                    ref var profile = ref profileElements.ElementAt(i);

                    if (profile.SpawnCount.Value <= 0) continue;

                    profile.SpawnDuration.DurationCounterSeconds += this.DeltaTime;

                    if (profile.SpawnDuration.DurationCounterSeconds >= profile.SpawnDuration.SpawnDurationSeconds)
                    {
                        profile.SpawnDuration.DurationCounterSeconds = 0;
                        profile.CanSpawnState = true;
                    }
                }
            }
        }


    }
}