using Components.GameEntity;
using Components.Player;
using Components.Player.Misc;
using Systems.Baking.Misc;
using Unity.Collections;
using Unity.Entities;

namespace Systems.Baking.Player.Misc
{
    [WorldSystemFilter(WorldSystemFilterFlags.BakingSystem)]
    [UpdateAfter(typeof(InstantiateEntityOnBakeSystem))]
    public partial class PlayerReactionConfigsHoldersBakingSystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerProfilesSOHolder
                    , BakedGameEntityProfileElement>()
                .Build();

            this.RequireForUpdate(this.query);

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;

            var profilesSOHolder = this.query.GetSingleton<PlayerProfilesSOHolder>();
            var bakedProfileElementArray = this.query.GetSingletonBuffer<BakedGameEntityProfileElement>().ToNativeArray(Allocator.Temp);

            int tempIndex = 0;

            foreach (var profile in profilesSOHolder.Value.Value.Profiles)
            {
                var targetEntity = bakedProfileElementArray[tempIndex].PrimaryEntity;
                if (targetEntity == Entity.Null) continue;

                this.EntityManager.AddSharedComponent(targetEntity, new PlayerReactionConfigsHolder
                {
                    Value = profile.Value.PlayerReactionConfigs,
                });

                tempIndex++;
            }

        }

    }

}