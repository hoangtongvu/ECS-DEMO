using Components.GameEntity;
using Components.Harvest;
using Systems.Baking.Misc;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Baking.Harvest
{
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    [UpdateAfter(typeof(InstantiateEntityOnBakeSystem))]
    public partial class HarvesteeProfileIdHoldersBakingSystem : SystemBase
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

            int tempIndex = 0;

            foreach (var profile in profilesSOHolder.Value.Value.Profiles)
            {
                var targetEntity = bakedProfileElementArray[tempIndex].PrimaryEntity;
                if (targetEntity == Entity.Null) continue;

                this.EntityManager.AddComponentData(targetEntity, new HarvesteeProfileIdHolder
                {
                    Value = profile.Key,
                });

                tempIndex++;
            }

            bakedProfileElementArray.Dispose();

        }

    }

}