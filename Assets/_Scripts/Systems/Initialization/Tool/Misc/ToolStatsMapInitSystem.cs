using Components.GameEntity;
using Components.Tool;
using Components.Tool.Misc;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Tool.Misc
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class ToolStatsMapInitSystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    ToolProfilesSOHolder
                    , BakedGameEntityProfileElement>()
                .Build();

            this.RequireForUpdate(this.query);

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var profiles = this.query.GetSingleton<ToolProfilesSOHolder>().Value.Value.Profiles;
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            var map = new ToolStatsMap
            {
                Value = new(profiles.Count, Allocator.Persistent),
            };

            foreach (var profile in profiles)
            {
                map.Value.Add(profile.Key, profile.Value.ToolStatsInSO.ToToolStats());
            }

            su.AddOrSetComponentData(map);

        }

    }

}