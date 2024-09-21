using Components.GameResource;
using Unity.Burst;
using Unity.Entities;
using Utilities;


namespace Systems.Initialization.GameResource
{

    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct ResourceItemPrefabInitSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ResourceItemICD>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            var resourceItemEntity = SystemAPI.GetSingletonEntity<ResourceItemICD>();
            state.EntityManager.AddComponent<Prefab>(resourceItemEntity);

            SingletonUtilities.GetInstance(state.EntityManager)
                .AddOrSetComponentData(new ResourceItemEntityHolder
                {
                    Value = resourceItemEntity,
                });

        }


    }
}