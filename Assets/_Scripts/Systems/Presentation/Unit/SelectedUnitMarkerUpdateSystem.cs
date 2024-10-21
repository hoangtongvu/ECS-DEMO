using Unity.Entities;
using Components.Unit.UnitSelection;
using Components.Unit;
using Unity.Transforms;
using Unity.Burst;

namespace Systems.Presentation.Unit
{

    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [UpdateAfter(typeof(SelectedUnitMarkerSpawnSystem))]
    [BurstCompile]
    public partial struct SelectedUnitMarkerUpdateSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<
                    LocalTransform
                    , SelectedUnitMarkerHolder
                    , UnitSelectedTag>()
                .Build();

            state.RequireForUpdate(query);

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {

            foreach (var (transformRef, selectedUnitMarkerHolderRef) in
                SystemAPI.Query<
                    RefRO<LocalTransform>
                    , RefRO<SelectedUnitMarkerHolder>>()
                    .WithAll<UnitSelectedTag>())
            {
                var markerTransformRef = SystemAPI.GetComponentRW<LocalTransform>(selectedUnitMarkerHolderRef.ValueRO.Value);

                markerTransformRef.ValueRW.Position = new(
                    transformRef.ValueRO.Position.x
                    , markerTransformRef.ValueRO.Position.y
                    , transformRef.ValueRO.Position.z);

            }

        }


    }
}