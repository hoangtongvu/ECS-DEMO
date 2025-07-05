using Components.GameEntity.EntitySpawning;
using Components.Tool;
using Core.Utilities.Extensions;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Utilities.Extensions.GameEntity.EntitySpawning;

namespace Systems.Simulation.UnitAndTool.ToolPick
{
    [UpdateInGroup(typeof(ToolPickHandleSystemGroup), OrderLast = true)]
    [UpdateBefore(typeof(UnmarkToolCanBePickedSystem))]
    [BurstCompile]
    public partial struct HandleToolOnToolPickSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    LocalTransform
                    , SpawnerEntityHolder
                    , ToolPickerEntity>()
                .WithAll<
                    DerelictToolTag
                    , CanBePickedTag>()
                .Build();

            state.RequireForUpdate(query0);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (transformRef, spawnerEntityHolderRef, toolPickerEntityRef, toolEntity) in SystemAPI
                .Query<
                    RefRW<LocalTransform>
                    , RefRW<SpawnerEntityHolder>
                    , RefRO<ToolPickerEntity>>()
                .WithAll<
                    DerelictToolTag
                    , CanBePickedTag>()
                .WithEntityAccess())
            {
                this.HandleTool(
                    ref state
                    , ref transformRef.ValueRW
                    , ref spawnerEntityHolderRef.ValueRW
                    , ecb
                    , in toolPickerEntityRef.ValueRO.Value
                    , in toolEntity);

            }

            ecb.Playback(state.EntityManager);

        }

        [BurstCompile]
        private void HandleTool(
            ref SystemState state
            , ref LocalTransform transform
            , ref SpawnerEntityHolder spawnerEntityHolder
            , EntityCommandBuffer ecb
            , in Entity pickerEntity
            , in Entity toolEntity)
        {
            // Remove Gravity of tool
            ecb.AddComponent(toolEntity, new PhysicsGravityFactor
            {
                Value = 0
            });

            // Hide tool out of view
            var pickerPos = SystemAPI.GetComponent<LocalTransform>(pickerEntity).Position;
            transform.Position = pickerPos.Add(y: 100f);
            transform.Scale = 0;
            transform.Rotation = quaternion.identity;

            spawnerEntityHolder.Value = Entity.Null;

            ecb.RemoveComponent<DerelictToolTag>(toolEntity);

        }

    }

}