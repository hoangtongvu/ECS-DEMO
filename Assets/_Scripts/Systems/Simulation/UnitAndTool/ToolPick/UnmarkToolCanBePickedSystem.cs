using Components.Tool;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Simulation.UnitAndTool.ToolPick
{
    [UpdateInGroup(typeof(ToolPickHandleSystemGroup), OrderLast = true)]
    [BurstCompile]
    public partial struct UnmarkToolCanBePickedSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    CanBePickedTag
                    , ToolPickerEntity>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (canBePickedTagRef, toolPickerEntityRef) in SystemAPI
                .Query<
                    EnabledRefRW<CanBePickedTag>
                    , RefRW<ToolPickerEntity>>())
            {
                canBePickedTagRef.ValueRW = false;
                toolPickerEntityRef.ValueRW.Value = Entity.Null;
            }

        }

    }

}