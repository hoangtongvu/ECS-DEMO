using Components.GameResource;
using Components.GameResource.Misc;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.GameResource.Misc
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class MaxQuantityPerStackMapInitSystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    ResourceProfilesSOHolder>()
                .Build();

            this.RequireForUpdate(this.query);
        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var profiles = this.query.GetSingleton<ResourceProfilesSOHolder>().Value.Value.Profiles;
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            int length = profiles.Count;
            var map = new MaxQuantityPerStackMap
            {
                Value = new(length, Allocator.Persistent),
            };

            foreach ( var profile in profiles )
            {
                map.Value.Add(profile.Key, profile.Value.MaxQuantityPerStack);
            }

            su.AddOrSetComponentData(map);
        }

    }

}