using Components.GameEntity;
using Components.Unit;
using Components.Unit.Reaction;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Unit.Reaction
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class UnitReactionConfigsMapInitSystem : SystemBase
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

            var profiles = this.query.GetSingleton<UnitProfilesSOHolder>().Value.Value.Profiles;
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            int profileCount = profiles.Count;

            var commandSourceMap = new UnitReactionConfigsMap
            {
                Value = new(profileCount, Allocator.Persistent),
            };

            foreach (var pair in profiles)
            {
                commandSourceMap.Value.Add(pair.Key, pair.Value.UnitReactionConfigs);
            }

            su.AddOrSetComponentData(commandSourceMap);

        }

    }

}