using Unity.Entities;
using Components;
using Unity.Burst;
using Components.MyEntity;

namespace Systems.Simulation.MyEntity
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
                    , DistanceToTarget
                    , CanInteractEntityTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Clear components.
            foreach (var (tagRef, targetEntityRef) in SystemAPI.Query<EnabledRefRW<CanInteractEntityTag>, RefRW<TargetEntity>>())
            {
                tagRef.ValueRW = false;
                targetEntityRef.ValueRW.Value = Entity.Null;
            }

            foreach (var (distanceToTargetRef, targetEntityRef, entity) in
                SystemAPI.Query<
                    RefRO<DistanceToTarget>
                    , RefRO<TargetEntity>>()
                    .WithDisabled<CanInteractEntityTag>()
                    .WithEntityAccess())
            {
                const float interactionRadius = 2f;// TODO: Replace this with an ICD

                if (targetEntityRef.ValueRO.Value == Entity.Null) continue;
                if (distanceToTargetRef.ValueRO.CurrentDistance > interactionRadius) continue;
                //UnityEngine.Debug.Log(distanceToTargetRef.ValueRO.CurrentDistance);
                SystemAPI.SetComponentEnabled<CanInteractEntityTag>(entity, true);
            }

        }


    }
}