using Components.GameEntity.InteractableActions;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Simulation.GameEntity.InteractableActions
{
    [UpdateInGroup(typeof(ActionsContainerUpdateSystemGroup), OrderLast = true)]
    [BurstCompile]
    public partial struct ActionsContainerCanUpdateTagClearSystem : ISystem
    {
        private EntityQuery query0;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query0 = SystemAPI.QueryBuilder()
                .WithAll<ActionsContainerUI_CD.CanUpdate>()
                .Build();

            state.RequireForUpdate(this.query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.SetComponentEnabled<ActionsContainerUI_CD.CanUpdate>(this.query0, false);
        }

    }

}