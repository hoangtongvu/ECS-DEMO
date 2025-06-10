using Components.GameEntity;
using Components.Harvest;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Harvest
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class HarvesteeResourceDropInfoMapInitSystem : SystemBase
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

            var resourceDropInfoMap = new HarvesteeResourceDropInfoMap
            {
                Value = new(10, Allocator.Persistent),
            };
            
            int tempIndex = 0;

            foreach (var profile in profilesSOHolder.Value.Value.Profiles)
            {
                var keyEntity = bakedProfileElementArray[tempIndex].PrimaryEntity;
                if (keyEntity == Entity.Null)
                    throw new System.NullReferenceException($"keyEntity for {nameof(HarvesteeResourceDropInfoMap)} equals to Entity.Null");

                resourceDropInfoMap.Value.Add(keyEntity, profile.Value.ResourceDropInfo);
                tempIndex++;
            }

            su.AddOrSetComponentData(resourceDropInfoMap);

        }

    }

}