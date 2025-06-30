using Components.GameEntity.InteractableActions;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Simulation.GameEntity.InteractableActions
{
    [UpdateInGroup(typeof(ActionsTriggeredSystemGroup), OrderFirst = true)]
    [BurstCompile]
    public partial struct NewlyActionTriggeredTagClearSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    NewlyActionTriggeredTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var tag in SystemAPI
                .Query<
                    EnabledRefRW<NewlyActionTriggeredTag>>())
            {
                tag.ValueRW = false;
            }

        }

    }

}