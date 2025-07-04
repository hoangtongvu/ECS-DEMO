using Components.Tool.Misc;
using Components.Unit;
using Components.Unit.RevertToBaseUnit;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using static Utilities.Helpers.UnitAndTool.UpgradeAndRevertJoblessUnit.UpgradeAndRevertJoblessUnitHelper;

namespace Systems.Initialization.UnitAndTool.RevertToBaseUnit.RevertArmedState
{
    [UpdateInGroup(typeof(RevertArmedStateSystemGroup))]
    [BurstCompile]
    public partial struct RevertOnPick_Weapon_System : ISystem
    {
        private EntityQuery query0;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            this.query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitToolHolder
                    , NeedRevertToBaseUnitTag>()
                .Build();

            state.RequireForUpdate(this.query0);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            var entities = new NativeList<Entity>(this.query0.CalculateEntityCount(), Allocator.Temp);

            foreach (var (unitToolHolderRef, entity) in SystemAPI
                .Query<
                    RefRO<UnitToolHolder>>()
                .WithAll<
                    NeedRevertToBaseUnitTag>()
                .WithEntityAccess())
            {
                bool toolIsWeapon = SystemAPI.HasComponent<IsWeaponTag>(unitToolHolderRef.ValueRO.Value);
                if (!toolIsWeapon) continue;

                entities.Add(entity);
            }

            RevertOnPick_Weapon(in em, entities.AsArray());

        }

    }

}