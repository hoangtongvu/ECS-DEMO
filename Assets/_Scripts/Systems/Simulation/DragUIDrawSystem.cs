using Unity.Entities;
using Unity.Mathematics;
using Components;
using Core.Utilities.Extensions;
using Unity.Transforms;
using Unity.Burst;

namespace Systems.Simulation
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct DragUIDrawSystem : ISystem
    {
        private float3 startPos;
        private EntityQuery dragSpriteQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.dragSpriteQuery = SystemAPI.QueryBuilder()
                .WithAllRW<LocalTransform, PostTransformMatrix>()
                .WithAll<DragSelectionSpriteTag>()
                .Build();

            state.RequireForUpdate(this.dragSpriteQuery);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.CompleteDependencyBeforeRW<LocalTransform>();
            state.EntityManager.CompleteDependencyBeforeRW<PostTransformMatrix>();

            var transformRef = this.dragSpriteQuery.GetSingletonRW<LocalTransform>();
            var postTransformMatrixRef = this.dragSpriteQuery.GetSingletonRW<PostTransformMatrix>();

            var dragSelectionData = SystemAPI.GetSingleton<DragSelectionData>();

            if (!dragSelectionData.IsDragging)
            {
                transformRef.ValueRW.Scale = 0f;
                return;
            }

            this.startPos = dragSelectionData.StartWorldPos;

            float3 currentPos = dragSelectionData.CurrentWorldPos;

            float3 centerPos = (this.startPos + currentPos) / 2;

            float2 size =
                new float2(this.startPos.x, this.startPos.z) -
                new float2(currentPos.x, currentPos.z);

            // set sprite pos = centerPos.
            transformRef.ValueRW.Position = centerPos.Add(y: 0.05f);

            float3 tempScale = new(size.x, size.y, 1f); // Note: Set z = 0 make this sprite rotate, dunno why. So any value different than 0 is OK.

            transformRef.ValueRW.Scale = 100;

            postTransformMatrixRef.ValueRW.Value = float4x4.TRS(
                float3.zero
                , quaternion.EulerXYZ(float3.zero)
                , tempScale);

        }

    }

}