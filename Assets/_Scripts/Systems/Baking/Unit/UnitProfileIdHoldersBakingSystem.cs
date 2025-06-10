using Components.GameEntity;
using Components.Unit;
using Systems.Baking.Misc;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Baking.Unit
{
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    [UpdateAfter(typeof(InstantiateEntityOnBakeSystem))]
    public partial class UnitProfileIdHoldersBakingSystem : SystemBase
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

            int tempIndex = 0;

            foreach (var profile in profilesSOHolder.Value.Value.Profiles)
            {
                var targetEntity = bakedProfileElementArray[tempIndex].PrimaryEntity;
                if (targetEntity == Entity.Null) continue;

                this.EntityManager.AddComponentData(targetEntity, new UnitProfileIdHolder
                {
                    Value = profile.Key,
                });

                tempIndex++;
            }

        }

    }

}