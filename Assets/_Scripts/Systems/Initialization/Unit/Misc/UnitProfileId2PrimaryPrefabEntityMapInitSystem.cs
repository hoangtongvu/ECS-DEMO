using Components.GameEntity;
using Components.Unit;
using Components.Unit.Misc;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Unit.Misc
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class UnitProfileId2PrimaryPrefabEntityMapInitSystem : SystemBase
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

            var profilesSOHolder = this.query.GetSingleton<UnitProfilesSOHolder>();
            var bakedProfileElementArray = this.query.GetSingletonBuffer<BakedGameEntityProfileElement>().ToNativeArray(Allocator.Temp);
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            var map = new UnitProfileId2PrimaryPrefabEntityMap
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

            bakedProfileElementArray.Dispose();

        }

    }

}