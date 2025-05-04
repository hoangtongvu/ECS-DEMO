using Components.Harvest;
using Components.Misc.Presenter;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Initialization.Harvest
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [BurstCompile]
    public partial struct SpawnHarvesteePresentersSystem : ISystem
    {
        private EntityQuery query;
        private Random rand;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.rand = new(37);

            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    HarvesteeProfileIdToPresenterVariancesRangeMap
                    , HarvesteePresenterVariancesContainer>()
                .Build();

            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    HarvesteeProfileIdHolder
                    , NeedSpawnPresenterTag>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate(this.query);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            int entityCount = this.query.CalculateEntityCount();
            if (entityCount == 0) return;

            var profileIdToPresenterVariancesRangeMap = SystemAPI.GetSingleton<HarvesteeProfileIdToPresenterVariancesRangeMap>().Value;
            var presenterVariancesContainer = SystemAPI.GetSingleton<HarvesteePresenterVariancesContainer>().Value;
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var primaryEntityArray = this.query.ToEntityArray(Allocator.Temp);
            var presenterPrefabIndexArray = new NativeArray<int>(entityCount, Allocator.Temp);

            int tempIndex = 0;

            foreach (var (profileIdHolderRef, needSpawnPresenterTag, primaryEntity) in
                SystemAPI.Query<
                    RefRO<HarvesteeProfileIdHolder>
                    , EnabledRefRW<NeedSpawnPresenterTag>>()
                    .WithEntityAccess())
            {
                var key = profileIdHolderRef.ValueRO.Value;

                if (!profileIdToPresenterVariancesRangeMap.TryGetValue(key, out var range))
                    throw new KeyNotFoundException($"{nameof(HarvesteeProfileIdToPresenterVariancesRangeMap)} does not contain key: {key}");

                needSpawnPresenterTag.ValueRW = false;

                int upperBound = range.StartIndex + range.Count;

                presenterPrefabIndexArray[tempIndex] = this.rand.NextInt(range.StartIndex, upperBound);
                tempIndex++;

            }

            for (int i = 0; i < entityCount; i++)
            {
                Entity presenterEntity = state.EntityManager.Instantiate(presenterVariancesContainer[presenterPrefabIndexArray[i]]);
                this.SetVarianceAsChildEntity(ref state, ecb, primaryEntityArray[i], in presenterEntity);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
            primaryEntityArray.Dispose();
            presenterPrefabIndexArray.Dispose();

        }

        [BurstCompile]
        private void SetVarianceAsChildEntity(ref SystemState state, EntityCommandBuffer ecb, in Entity primaryEntity, in Entity childEntity)
        {
            ecb.AddComponent(childEntity, new Parent
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
                Value = childEntity,
            });

            if (SystemAPI.HasBuffer<LinkedEntityGroup>(childEntity))
            {
                var presenterLinkedSystemGroup = SystemAPI.GetBuffer<LinkedEntityGroup>(childEntity);

                int length = presenterLinkedSystemGroup.Length;
                if (length == 1) return;

                var presenterLinkedSystemGroupArray = presenterLinkedSystemGroup.ToNativeArray(Allocator.Temp);

                for (int j = 1; j < length; j++)
                {
                    linkedEntityGroup.Add(presenterLinkedSystemGroupArray[j]);
                }

                presenterLinkedSystemGroupArray.Dispose();
            }

        }

    }

}