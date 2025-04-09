using Unity.Entities;
using Components;
using Unity.Burst;
using Unity.Mathematics;
using Components.Misc.WorldMap;
using Components.GameEntity;

namespace Systems.Simulation.GameEntity
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct SetCanInteractFlagSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    TargetEntity
                    , AbsoluteDistanceXZToTarget
                    , CanInteractEntityTag
                    , CanMoveEntityTag>()
                .Build();

            state.RequireForUpdate(query0);
            state.RequireForUpdate<CellRadius>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            half cellRadius = SystemAPI.GetSingleton<CellRadius>().Value;

            // Clear components.
            foreach (var (tagRef, targetEntityRef) in SystemAPI.Query<EnabledRefRW<CanInteractEntityTag>, RefRW<TargetEntity>>())
            {
                tagRef.ValueRW = false;
                targetEntityRef.ValueRW.Value = Entity.Null;
            }

            foreach (var (absoluteDistanceXZToTargetRef, targetEntityRef, targetEntityWorldSquareRadiusRef, entity) in
                SystemAPI.Query<
                    RefRO<AbsoluteDistanceXZToTarget>
                    , RefRO<TargetEntity>
                    , RefRO<TargetEntityWorldSquareRadius>> ()
                    .WithDisabled<CanInteractEntityTag>()
                    .WithDisabled<CanMoveEntityTag>()
                    .WithEntityAccess())
            {
                if (targetEntityRef.ValueRO.Value == Entity.Null) continue;

                float interactRadius = targetEntityWorldSquareRadiusRef.ValueRO.Value + cellRadius * 2;

                if (absoluteDistanceXZToTargetRef.ValueRO.X > interactRadius) continue;
                if (absoluteDistanceXZToTargetRef.ValueRO.Z > interactRadius) continue;

                SystemAPI.SetComponentEnabled<CanInteractEntityTag>(entity, true);

            }

        }

    }

}