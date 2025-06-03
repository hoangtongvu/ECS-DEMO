using Components.GameEntity;
using Components.Unit;
using Components.Unit.Misc;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Unit.Misc
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class UnarmedUnitFleeTotalSecondsInitSystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitProfilesSOHolder
                    , BakedGameEntityProfileElement>()
                .Build();

            this.RequireForUpdate(this.query);

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var so = this.query.GetSingleton<UnitProfilesSOHolder>().Value.Value;
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            var unarmedUnitFleeTotalSeconds = new UnarmedUnitFleeTotalSeconds
            {
                Value = new(so.UnitGlobalConfigs.UnarmedUnitFleeTotalSeconds),
            };

            su.AddOrSetComponentData(unarmedUnitFleeTotalSeconds);

        }

    }

}