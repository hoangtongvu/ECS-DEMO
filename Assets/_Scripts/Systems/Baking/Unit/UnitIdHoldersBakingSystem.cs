using Components.GameEntity;
using Components.Unit;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Baking.Unit
{
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    public partial class UnitIdHoldersBakingSystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitProfilesSOHolder
                    , AfterBakedPrefabsElement>()
                .Build();

            this.RequireForUpdate(this.query);

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var profilesSOHolder = this.query.GetSingleton<UnitProfilesSOHolder>();
            var afterBakedPrefabsArray = this.query.GetSingletonBuffer<AfterBakedPrefabsElement>().ToNativeArray(Allocator.Temp);

            int tempIndex = 0;

            foreach (var profile in profilesSOHolder.Value.Value.Profiles)
            {
                var targetEntity = afterBakedPrefabsArray[tempIndex].PrimaryEntity;
                if (targetEntity == Entity.Null) continue;

                this.EntityManager.AddComponentData(targetEntity, new UnitIdHolder
                {
                    Value = profile.Key,
                });

                tempIndex++;
            }

            afterBakedPrefabsArray.Dispose();

        }

    }

}