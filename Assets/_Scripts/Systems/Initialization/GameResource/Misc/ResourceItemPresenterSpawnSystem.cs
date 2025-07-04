using Components.GameResource;
using Components.Misc.Presenter;
using Core.GameResource;
using Systems.Initialization.Misc.Presenter;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Systems.Initialization.GameResource.Misc
{
    [UpdateInGroup(typeof(NeedSpawnPresenterTagProcessSystemGroup))]
    [BurstCompile]
    public partial struct ResourceItemPresenterSpawnSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    ResourceItemICD
                    , NeedSpawnPresenterTag>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate<ResourceItemEntityHolder>();
            state.RequireForUpdate<ResourceItemSpawnCommandList>();
            state.RequireForUpdate<ResourceItemPresenterEntityPrefabMap>();

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

            }

            ecb.Playback(state.EntityManager);

        }

    }

}