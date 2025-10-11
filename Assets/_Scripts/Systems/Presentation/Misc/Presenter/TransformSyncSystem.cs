using Components.Misc.Presenter;
using Components.Misc.Presenter.TransformSync;
using Core.Misc.Presenter.TransformSync;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Jobs;

namespace Systems.Presentation.Misc.Presenter
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [BurstCompile]
    public partial class TransformSyncSystem : SystemBase
    {
        private EntityQuery entityQuery;

        protected override void OnCreate()
        {
            this.entityQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    LocalTransform
                    , PresenterHolder
                    , TransformAccessArrayIndex
                    , TransformSyncTypeHolder>()
                .WithNone<NeedSpawnPresenterTag>()
                .Build();

            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    LocalTransform
                    , PresenterHolder>()
                .WithNone<NeedSpawnPresenterTag>()
                .Build();

            this.RequireForUpdate(query0);
            this.RequireForUpdate<PresentersHolderScene>();
        }

        protected override void OnUpdate()
        {
            var presentersTransformAccessArrayGOHolder = SystemAPI.GetSingleton<PresentersTransformAccessArrayGOHolder>();
            var transformAccessArray = presentersTransformAccessArrayGOHolder.Value.Value.TransformAccessArray;

            var localTransforms = this.entityQuery.ToComponentDataArray<LocalTransform>(Allocator.TempJob);
            var syncTypes = this.entityQuery.ToComponentDataArray<TransformSyncTypeHolder>(Allocator.TempJob);
            var transformAccessArrayIndexes = this.entityQuery.ToComponentDataArray<TransformAccessArrayIndex>(Allocator.TempJob);

            int count = localTransforms.Length;
            var indexToIndexMap = new NativeArray<int>(transformAccessArray.length, Allocator.TempJob);

            var firstJob = new IndexesTransformJob()
            {
                Count = count,
                TransformAccessArrayIndexes = transformAccessArrayIndexes,
                IndexToIndexMap = indexToIndexMap,
            };

            var secondJob = new TransformSyncJob()
            {
                LocalTransforms = localTransforms,
                TransformSyncTypes = syncTypes,
                IndexToIndexMap = firstJob.IndexToIndexMap,
            };

            secondJob.Schedule(transformAccessArray, firstJob.Schedule());
        }

        [BurstCompile]
        private struct IndexesTransformJob : IJob
        {
            [ReadOnly] public int Count;

            [ReadOnly] [DeallocateOnJobCompletionAttribute]
            public NativeArray<TransformAccessArrayIndex> TransformAccessArrayIndexes;

            public NativeArray<int> IndexToIndexMap;

            [BurstCompile]
            public void Execute()
            {
                for (int i = 0; i < this.Count; i++)
                {
                    this.IndexToIndexMap[this.TransformAccessArrayIndexes[i].Value] = i;
                }
            }
        }

        [BurstCompile]
        private struct TransformSyncJob : IJobParallelForTransform
        {
            [ReadOnly] [DeallocateOnJobCompletionAttribute]
            public NativeArray<LocalTransform> LocalTransforms;

            [ReadOnly] [DeallocateOnJobCompletionAttribute]
            public NativeArray<TransformSyncTypeHolder> TransformSyncTypes;

            [ReadOnly] [DeallocateOnJobCompletionAttribute]
            public NativeArray<int> IndexToIndexMap;

            [BurstCompile]
            public void Execute(int index, TransformAccess transform)
            {
                if (!transform.isValid) return;

                int localTransformIndex = this.IndexToIndexMap[index];
                var localTransform = this.LocalTransforms[localTransformIndex];
                var syncType = this.TransformSyncTypes[localTransformIndex].valueOfTransformSyncType;

                switch (syncType)
                {
                    case TransformSyncType.Instant:
                        transform.SetPositionAndRotation(localTransform.Position, localTransform.Rotation);
                        break;
                    case TransformSyncType.Smooth:
                        var position = math.lerp(transform.position, localTransform.Position, 0.5f);
                        transform.SetPositionAndRotation(position, localTransform.Rotation);
                        break;
                }
            }

        }

    }

}