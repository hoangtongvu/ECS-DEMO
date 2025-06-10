using Components.GameEntity;
using Components.Tool;
using Components.Tool.Misc;
using Systems.Baking.Misc;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Baking.Tool.Misc
{
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    [UpdateAfter(typeof(InstantiateEntityOnBakeSystem))]
    public partial class ToolProfileIdHoldersBakingSystem : SystemBase
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

            int tempIndex = 0;

            foreach (var profile in profilesSOHolder.Value.Value.Profiles)
            {
                var targetEntity = bakedProfileElementArray[tempIndex].PrimaryEntity;
                if (targetEntity == Entity.Null) continue;

                this.EntityManager.AddComponentData(targetEntity, new ToolProfileIdHolder
                {
                    Value = profile.Key,
                });

                tempIndex++;
            }

        }

    }

}