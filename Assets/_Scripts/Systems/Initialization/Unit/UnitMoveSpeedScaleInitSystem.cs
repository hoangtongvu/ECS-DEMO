using Components.GameEntity;
using Components.Unit;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Unit
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class UnitMoveSpeedScaleInitSystem : SystemBase
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

            var profilesSO = this.query.GetSingleton<UnitProfilesSOHolder>().Value.Value;
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            su.AddOrSetComponentData(new UnitMoveSpeedScale
            {
                Value = profilesSO.UnitGlobalConfigs.MoveSpeedScale,
            });

        }

    }

}