using Components.GameEntity;
using Components.Harvest;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Harvest
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class HarvesteePrefabEntityMapInitSystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    HarvesteeProfilesSOHolder
                    , BakedGameEntityProfileElement>()
                .Build();

            this.RequireForUpdate(this.query);

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
            var profilesSOHolder = this.query.GetSingleton<HarvesteeProfilesSOHolder>();
            var bakedProfileElementArray = this.query.GetSingletonBuffer<BakedGameEntityProfileElement>().ToNativeArray(Allocator.Temp);
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            var prefabMap = new HarvesteePrefabEntityMap
            {
                Value = new(15, Allocator.Persistent),
            };

            int tempIndex = 0;

            foreach (var profile in profilesSOHolder.Value.Value.Profiles)
            {
                var primaryEntity = bakedProfileElementArray[tempIndex].PrimaryEntity;
                if (primaryEntity == Entity.Null) continue;

                prefabMap.Value.Add(profile.Key, primaryEntity);
                tempIndex++;

            }

            bakedProfileElementArray.Dispose();
            su.AddOrSetComponentData(prefabMap);

        }

    }

}