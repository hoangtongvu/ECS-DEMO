using Unity.Entities;
using Unity.Burst;
using Components.GameResource;
using Core.GameResource;
using Unity.Transforms;
using Components.Misc.Presenter;
using Unity.Collections;

namespace Systems.Simulation.GameResource
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct ResourceItemPresenterSpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ResourceItemEntityHolder>();
            state.RequireForUpdate<ResourceItemSpawnCommandList>();
            state.RequireForUpdate<NeedSpawnPresenterTag>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var presenterEntityPrefabMap = SystemAPI.GetSingleton<ResourceItemPresenterEntityPrefabMap>().Value;
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var id = new ResourceProfileId
            {
                VariantIndex = 0,
            };

            foreach (var (resourceItemICDRef, primaryEntity) in
                SystemAPI.Query<
                    RefRO<ResourceItemICD>>()
                    .WithAll<NeedSpawnPresenterTag>()
                    .WithEntityAccess())
            {
                id.ResourceType = resourceItemICDRef.ValueRO.ResourceType;
                var presenterEntityPrefab = presenterEntityPrefabMap[id];

                var newPresenter = ecb.Instantiate(presenterEntityPrefab);

                ecb.AddComponent(newPresenter, new Parent
                {
                    Value = primaryEntity,
                });

                var linkedEntityGroup = ecb.AddBuffer<LinkedEntityGroup>(primaryEntity);
                linkedEntityGroup.Add(new()
                {
                    Value = primaryEntity,
                });

                linkedEntityGroup.Add(new()
                {
                    Value = newPresenter,
                });

                ecb.RemoveComponent<NeedSpawnPresenterTag>(primaryEntity);

            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();

        }

    }

}