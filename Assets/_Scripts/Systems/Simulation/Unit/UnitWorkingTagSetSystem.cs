using Unity.Entities;
using Unity.Burst;
using Components.MyEntity;
using Components.Unit;

namespace Systems.Simulation.Unit
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [BurstCompile]
    public partial struct UnitWorkingTagSetSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    InteractingEntity
                    , IsUnitWorkingTag>()
                .Build();

            state.RequireForUpdate(query0);
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Note: make this system updated after all systems that assign InteractingEntity
            foreach (var (interactingEntityRef, isUnitWorkingTag) in
                SystemAPI.Query<
                    RefRO<InteractingEntity>
                    , EnabledRefRW<IsUnitWorkingTag>>()
                    .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
            {
                isUnitWorkingTag.ValueRW = interactingEntityRef.ValueRO.Value != Entity.Null;
            }

            

        }


    }
}