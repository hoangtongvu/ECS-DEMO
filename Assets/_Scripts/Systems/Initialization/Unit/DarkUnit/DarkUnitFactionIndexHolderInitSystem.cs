using Components.Unit.DarkUnit;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Unit.DarkUnit
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class DarkUnitFactionIndexHolderInitSystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    DarkUnitConfigsSOHolder>()
                .Build();

            this.RequireForUpdate(this.query);
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var configsSO = this.query.GetSingleton<DarkUnitConfigsSOHolder>().Value.Value;
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            su.AddOrSetComponentData(new DarkUnitFactionIndexHolder
            {
                Value = new()
                {
                    Value = configsSO.DefaultDarkUnitFactionIndex,
                }
            });

        }

    }

}