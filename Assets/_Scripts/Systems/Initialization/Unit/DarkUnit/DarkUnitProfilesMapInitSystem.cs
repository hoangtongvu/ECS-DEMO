using Components.Unit.DarkUnit;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Unit.DarkUnit
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class DarkUnitProfilesMapInitSystem : SystemBase
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

            var profiles = this.query.GetSingleton<DarkUnitConfigsSOHolder>().Value.Value.DarkUnitProfiles;
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            var map = new DarkUnitProfileMap
            {
                Value = new(profiles.Count, Allocator.Persistent),
            };

            foreach (var profile in profiles)
            {
                map.Value.Add(profile.Key, profile.Value);
            }

            su.AddOrSetComponentData(map);

        }

    }

}