using Components.GameEntity;
using Components.Harvest;
using Components.Harvest.HarvesteeHp;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Harvest.HarvesteeHp
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class HarvesteeMaxHpMapInitSystem : SystemBase
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

            var maxHpMap = new HarvesteeMaxHpMap
            {
                Value = new(10, Allocator.Persistent),
            };
            
            int tempIndex = 0;

            foreach (var profile in profilesSOHolder.Value.Value.Profiles)
            {
                var keyEntity = bakedProfileElementArray[tempIndex].PrimaryEntity;
                if (keyEntity == Entity.Null)
                    throw new System.NullReferenceException($"keyEntity for {nameof(HarvesteeMaxHpMap)} equals to Entity.Null");

                maxHpMap.Value.Add(keyEntity, profile.Value.MaxHp);
                tempIndex++;
            }

            su.AddOrSetComponentData(maxHpMap);
            bakedProfileElementArray.Dispose();

        }

    }

}