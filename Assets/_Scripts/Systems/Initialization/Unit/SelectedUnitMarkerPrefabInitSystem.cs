using Components.Unit;
using Unity.Burst;
using Unity.Entities;
using Utilities;


namespace Systems.Initialization.Unit
{

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct SelectedUnitMarkerPrefabInitSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SelectedUnitMarkerTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var prefabEntity = SystemAPI.GetSingletonEntity<SelectedUnitMarkerTag>();
            state.EntityManager.AddComponent<Prefab>(prefabEntity);

            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new SelectedUnitMarkerPrefab
                {
                    Value = prefabEntity,
                });

        }


    }
}