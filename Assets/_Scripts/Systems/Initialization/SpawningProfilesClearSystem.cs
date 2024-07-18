using Unity.Entities;
using Unity.Scenes;
using Components.MyEntity.EntitySpawning;
using Unity.Burst;

namespace Systems.Initialization
{

    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    [UpdateBefore(typeof(SceneSystemGroup))]
    [BurstCompile]
    public partial struct SpawningProfilesClearSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<EntitySpawningProfileElement>()
                .Build();

            state.RequireForUpdate(query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            foreach (var spawningProfiles in
                SystemAPI.Query<DynamicBuffer<EntitySpawningProfileElement>>())
            {
                int length = spawningProfiles.Length;
                for (int i = 0; i < length; i++)
                {
                    ref var profile = ref spawningProfiles.ElementAt(i);
                    this.ClearProfileFields(ref profile);
                }

            }

        }

        [BurstCompile]
        private void ClearProfileFields(ref EntitySpawningProfileElement profile)
        {
            profile.SpawnCount.ValueChanged = false;
        }
    }
}