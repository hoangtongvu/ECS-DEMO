using Components.GameEntity.Misc;
using Unity.Entities;
using Systems.Initialization.GameEntity.Misc;
using Unity.Burst;
using Components.Unit;
using Components.Unit.Misc;
using Unity.Collections;

namespace Systems.Initialization.Unit.Misc
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateBefore(typeof(DestroyEntityWithTagSystem))]
    [BurstCompile]
    public partial struct DestroyTargetPosMarker_OnUnitDead_System : ISystem
    {
        private EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    NeedDestroyEntityTag
                    , TargetPosMarkerHolder>()
                .WithAll<UnitTag>()
                .Build();

            state.RequireForUpdate(this.query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var markerHolders = this.query.ToComponentDataArray<TargetPosMarkerHolder>(Allocator.Temp);
            int count = markerHolders.Length;
            var em = state.EntityManager;

            for (int i = 0; i < count; i++)
            {
                em.DestroyEntity(markerHolders[i].Value);
            }

        }

    }

}