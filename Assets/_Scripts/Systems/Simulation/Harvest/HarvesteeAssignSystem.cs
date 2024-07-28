using Unity.Entities;
using Unity.Burst;
using Components.MyEntity;
using Systems.Simulation.MyEntity;
using Components.Harvest;
using Core.Harvest;

namespace Systems.Simulation.Harvest
{

    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateAfter(typeof(SetCanInteractFlagSystem))]
    [BurstCompile]
    public partial struct HarvesteeAssignSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    TargetEntity
                    , CanInteractEntityTag>()
                .Build();

            state.RequireForUpdate(query0);

            state.RequireForUpdate<HarvesteeTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (targetEntityRef, harvesteeHealthIdOnUnit) in
                SystemAPI.Query<
                    RefRO<TargetEntity>
                    , RefRW<HarvesteeHealthId>>()
                    .WithAll<
                        CanInteractEntityTag
                        , HarvesterICD>())
            {
                var targetEntity = targetEntityRef.ValueRO.Value;

                if (!SystemAPI.HasComponent<HarvesteeTag>(targetEntity)) continue;

                var idOnHarvesteeRef = SystemAPI.GetComponentRW<HarvesteeHealthId>(targetEntity);
                if (!idOnHarvesteeRef.IsValid)
                {
                    UnityEngine.Debug.LogError("Target has HarvesteeTag but doesn't have HarvesteeHealthId");
                    continue;
                }

                var newId = new HarvesteeHealthId
                {
                    Value = new HealthId
                    {
                        Index = targetEntity.Index,
                        Version = targetEntity.Version,
                    }
                };

                this.AssignNewId(ref harvesteeHealthIdOnUnit.ValueRW, newId);
                this.AssignNewId(ref idOnHarvesteeRef.ValueRW, newId);

                UnityEngine.Debug.Log($"Interacted {targetEntity}.");
            }

        }

        [BurstCompile]
        private void AssignNewId(
            ref HarvesteeHealthId currentId
            , in HarvesteeHealthId newId) => currentId = newId;

    }
}