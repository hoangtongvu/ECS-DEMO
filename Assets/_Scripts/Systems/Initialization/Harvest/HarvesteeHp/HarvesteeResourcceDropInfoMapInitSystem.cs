using Components.GameEntity;
using Components.Harvest;
using Components.Harvest.HarvesteeHp;
using Systems.Initialization.GameEntity.EntitySpawning.SpawningProfiles;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Harvest.HarvesteeHp
{
    [UpdateInGroup(typeof(AddSpawningProfilesSystemGroup))]
    public partial class HarvesteeResourcceDropInfoMapInitSystem : SystemBase
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

            var resourceDropInfoMap = new HarvesteeResourcceDropInfoMap
            {
                Value = new(10, Allocator.Persistent),
            };
            
            int tempIndex = 0;

            foreach (var profile in profilesSOHolder.Value.Value.Profiles)
            {
                var keyEntity = bakedProfileElementArray[tempIndex].PrimaryEntity;
                if (keyEntity == Entity.Null)
                    throw new System.NullReferenceException($"keyEntity for {nameof(HarvesteeMaxHpMap)} equals to Entity.Null");

                resourceDropInfoMap.Value.Add(keyEntity, profile.Value.ResourceDropInfo);
                tempIndex++;
            }

            su.AddOrSetComponentData(resourceDropInfoMap);
            bakedProfileElementArray.Dispose();

        }

    }

}