using Unity.Entities;
using Unity.Collections;
using Unity.Burst;
using Components.GameEntity.Reaction;
using Components.GameEntity.Attack;

namespace Systems.Simulation.Player.Attack
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [BurstCompile]
    public partial struct ResetAttackEventTriggeredTagOnAttackEndedSystem : ISystem
    {
        private EntityQuery query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    AttackReaction.EndedTag>()
                .WithAll<
                    AttackEventTriggeredTag>()
                .Build();

            state.RequireForUpdate(this.query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var entities = this.query.ToEntityArray(Allocator.Temp);
            int length = entities.Length;

            if (length == 0) return;

            for (int i = 0; i < length; i++)
            {
                SystemAPI.SetComponentEnabled<AttackEventTriggeredTag>(entities[i], false);
            }

        }

    }

}
