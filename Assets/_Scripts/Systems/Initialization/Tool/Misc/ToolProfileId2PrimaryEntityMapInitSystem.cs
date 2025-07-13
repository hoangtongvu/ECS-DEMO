using Components.GameEntity;
using Components.Tool;
using Components.Tool.Misc;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Tool.Misc
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class ToolProfileId2PrimaryEntityMapInitSystem : SystemBase
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

            var profilesSOHolder = this.query.GetSingleton<ToolProfilesSOHolder>();
            var bakedProfileElementArray = this.query.GetSingletonBuffer<BakedGameEntityProfileElement>().ToNativeArray(Allocator.Temp);
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            var map = new ToolProfileId2PrimaryEntityMap
            {
                Value = new(bakedProfileElementArray.Length, Allocator.Persistent),
            };

            int tempIndex = 0;

            foreach (var profile in profilesSOHolder.Value.Value.Profiles)
            {
                var targetEntity = bakedProfileElementArray[tempIndex].PrimaryEntity;
                tempIndex++;

                if (targetEntity == Entity.Null) continue;

                map.Value.Add(profile.Key, targetEntity);

            }

            su.AddOrSetComponentData(map);

        }

    }

}