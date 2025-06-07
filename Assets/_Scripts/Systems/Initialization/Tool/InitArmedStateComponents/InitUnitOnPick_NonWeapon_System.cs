using Components.GameEntity.Misc;
using Components.Tool.Misc;
using Components.Unit;
using Components.Unit.Misc;
using Core.GameEntity.Misc;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Initialization.Tool.InitArmedStateComponents
{
    [UpdateInGroup(typeof(InitArmedStateComponentsSystemGroup))]
    [BurstCompile]
    public partial struct InitUnitOnPick_NonWeapon_System : ISystem
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

                if (toolIsWeapon) return;

                this.ECB.SetComponent(entityIndexInQuery, unitEntity, new ArmedStateHolder
                {
                    Value = ArmedState.False,
                });

            }

        }

    }

}