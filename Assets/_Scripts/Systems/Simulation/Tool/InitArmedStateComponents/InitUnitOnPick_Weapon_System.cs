using Components.Tool.Misc;
using Components.Unit;
using Components.Unit.Misc;
using Core.Unit.Misc;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Simulation.Tool.InitArmedStateComponents
{
    [UpdateInGroup(typeof(InitArmedStateComponentsSystemGroup))]
    [BurstCompile]
    public partial struct InitUnitOnPick_Weapon_System : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitToolHolder
                    , NeedInitArmedStateComponentsTag>()
                .Build();

            state.RequireForUpdate(query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            new InitArmedStateComponentsJob
            {
                ECB = ecb.AsParallelWriter(),
                IsWeaponTagLookup = SystemAPI.GetComponentLookup<IsWeaponTag>(),
            }.ScheduleParallel();

        }

        [WithAll(typeof(NeedInitArmedStateComponentsTag))]
        [BurstCompile]
        private partial struct InitArmedStateComponentsJob : IJobEntity
        {
            public EntityCommandBuffer.ParallelWriter ECB;

            [ReadOnly]
            public ComponentLookup<IsWeaponTag> IsWeaponTagLookup;

            [BurstCompile]
            void Execute(
                in UnitToolHolder unitToolHolder
                , Entity unitEntity
                , [EntityIndexInQuery] int entityIndexInQuery)
            {
                bool toolIsWeapon = this.IsWeaponTagLookup.HasComponent(unitToolHolder.Value);

                if (!toolIsWeapon) return;

                this.AddComponents(in entityIndexInQuery, in unitEntity);

                this.ECB.RemoveComponent<NeedInitArmedStateComponentsTag>(entityIndexInQuery, unitEntity);

            }

            [BurstCompile]
            private void AddComponents(in int entityIndexInQuery, in Entity entity)
            {
                this.ECB.SetComponent(entityIndexInQuery, entity, new ArmedStateHolder
                {
                    Value = ArmedState.True,
                });

                this.ECB.RemoveComponent<IsUnarmedUnitTag>(entityIndexInQuery, entity);
                this.ECB.AddComponent<IsArmedUnitTag>(entityIndexInQuery, entity);

            }

        }

    }

}