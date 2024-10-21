using Unity.Entities;
using Components.Unit.UnitSelection;
using Unity.Mathematics;
using Core.Utilities.Extensions;
using Components.Unit;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Physics;
using Core;

namespace Systems.Presentation.Unit
{

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [BurstCompile]
    public partial struct SelectedUnitMarkerSpawnSystem : ISystem
    {

        private EntityQuery unitQuery;


        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    LocalTransform
                    , SelectedUnitMarkerHolder
                    , NewlySelectedUnitTag
                    , NewlyDeselectedUnitTag>()
                .Build();

            this.unitQuery = SystemAPI.QueryBuilder()
                .WithAll<
                    LocalTransform
                    , SelectedUnitMarkerHolder
                    , NewlySelectedUnitTag>()
                .Build();

            state.RequireForUpdate(query);
            state.RequireForUpdate<SelectedUnitMarkerPrefab>();

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
            var selectedUnitMarkerPrefab = SystemAPI.GetSingleton<SelectedUnitMarkerPrefab>();

            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);


            foreach (var (transformRef, selectedUnitMarkerHolderRef) in
                SystemAPI.Query<
                    RefRO<LocalTransform>
                    , RefRW<SelectedUnitMarkerHolder>>()
                    .WithAll<NewlySelectedUnitTag>())
            {

                this.SpawnMarker(
                    ref state
                    , in physicsWorld
                    , in selectedUnitMarkerPrefab.Value
                    , transformRef.ValueRO.Position
                    , ref selectedUnitMarkerHolderRef.ValueRW);

            }

            foreach (var selectedUnitMarkerHolderRef in
                SystemAPI.Query<
                    RefRW<SelectedUnitMarkerHolder>>()
                    .WithAll<NewlyDeselectedUnitTag>())
            {

                this.DestroyMarker(
                    in ecb
                    , ref selectedUnitMarkerHolderRef.ValueRW);

            }


        }

        [BurstCompile]
        private void SpawnMarker(
            ref SystemState state
            , in PhysicsWorldSingleton physicsWorld
            , in Entity markerPrefab
            , float3 rawPos
            , ref SelectedUnitMarkerHolder selectedUnitMarkerHolder)
        {

            bool hit = this.CastRay(
                in physicsWorld
                , rawPos
                , out var raycastHit);

            if (!hit) return;

            float3 spawnPos = raycastHit.Position.Add(y: 0.05f);
            var markerEntity = state.EntityManager.Instantiate(markerPrefab);

            var transformRef = SystemAPI.GetComponentRW<LocalTransform>(markerEntity);
            transformRef.ValueRW.Position = spawnPos;

            selectedUnitMarkerHolder.Value = markerEntity;

        }

        [BurstCompile]
        private bool CastRay(
            in PhysicsWorldSingleton physicsWorld
            , float3 startPos
            , out Unity.Physics.RaycastHit raycastHit)
        {
            float3 rayStart = startPos;
            float3 rayEnd = startPos.Add(y: -500f);

            RaycastInput raycastInput = new()
            {
                Start = rayStart,
                End = rayEnd,
                Filter = new CollisionFilter
                {
                    BelongsTo = (uint)CollisionLayer.Ground,
                    CollidesWith = (uint)CollisionLayer.Ground,
                },
            };

            return physicsWorld.CastRay(raycastInput, out raycastHit);
        }

        [BurstCompile]
        private void DestroyMarker(
            in EntityCommandBuffer ecb
            , ref SelectedUnitMarkerHolder selectedUnitMarkerHolder)
        {
            ecb.DestroyEntity(selectedUnitMarkerHolder.Value);
            selectedUnitMarkerHolder.Value = Entity.Null;
        }

        //Note: Tried Compare this with normal foreach (Main thread) way and hardly saw the difference in performance.
        [BurstCompile]
        private void ScheduleBatchJob(
            ref SystemState state
            , Entity selectedUnitMarkerPrefab)
        {
            if (!this.unitQuery.IsEmpty)
            {
                int spawnCount = this.unitQuery.CalculateEntityCount();
                var spawnedMarkers = state.EntityManager.Instantiate(selectedUnitMarkerPrefab, spawnCount, state.WorldUpdateAllocator);
                state.Dependency = default;

                var transforms = unitQuery.ToComponentDataArray<LocalTransform>(state.WorldUpdateAllocator);

                state.Dependency = new InitMarkerJob
                {
                    MarkerEntities = spawnedMarkers,
                    UnitEntities = this.unitQuery.ToEntityArray(state.WorldUpdateAllocator),
                    UnitTransforms = transforms,
                    TransformLookup = SystemAPI.GetComponentLookup<LocalTransform>(),
                    SelectedUnitMarkerHolderLookup = SystemAPI.GetComponentLookup<SelectedUnitMarkerHolder>(),
                }.ScheduleParallel(spawnCount, 64, state.Dependency);

            }

        }

        [BurstCompile]
        private struct InitMarkerJob : IJobParallelForBatch
        {
            [ReadOnly]
            public NativeArray<Entity> MarkerEntities;

            [ReadOnly]
            public NativeArray<LocalTransform> UnitTransforms;

            [ReadOnly]
            public NativeArray<Entity> UnitEntities;

            [NativeDisableParallelForRestriction]
            public ComponentLookup<LocalTransform> TransformLookup;

            [NativeDisableParallelForRestriction]
            public ComponentLookup<SelectedUnitMarkerHolder> SelectedUnitMarkerHolderLookup;

            [BurstCompile]
            void IJobParallelForBatch.Execute(int startIndex, int count)
            {
                int length = startIndex + count;

                for (int i = startIndex; i < length; i++)
                {
                    var markerEntity = this.MarkerEntities[i];
                    var unitEntity = this.UnitEntities[i];
                    var unitTransform = this.UnitTransforms[i];

                    var markerTransformRef = this.TransformLookup.GetRefRWOptional(markerEntity);
                    markerTransformRef.ValueRW.Position = unitTransform.Position.Add(y: 0.05f);

                    var selectedUnitMarkerHolderRef = this.SelectedUnitMarkerHolderLookup.GetRefRWOptional(unitEntity);
                    selectedUnitMarkerHolderRef.ValueRW.Value = markerEntity;

                }

            }
        }

    }
}