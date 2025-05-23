using Components.Unit;
using Unity.Burst;
using Unity.Entities;
using Utilities;


namespace Systems.Initialization.Unit
{

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct TargetPosMarkerPrefabInitSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var prefabEntityQuery = SystemAPI.QueryBuilder()
                .WithAll<TargetPosMarkerTag>()
                .WithOptions(EntityQueryOptions.IncludePrefab)
                .Build();

            state.RequireForUpdate(prefabEntityQuery);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var prefabEntityQuery = SystemAPI.QueryBuilder()
                .WithAll<TargetPosMarkerTag>()
                .WithOptions(EntityQueryOptions.IncludePrefab)
                .Build();

            var prefabEntity = prefabEntityQuery.GetSingletonEntity();

            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new TargetPosMarkerPrefab
                {
                    Value = prefabEntity,
                });

        }


    }
}