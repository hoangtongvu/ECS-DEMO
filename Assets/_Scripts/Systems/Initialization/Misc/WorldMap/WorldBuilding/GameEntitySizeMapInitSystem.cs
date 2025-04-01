using Components.GameEntity;
using Components.Misc.WorldMap.WorldBuilding;
using Core.Misc.WorldMap.WorldBuilding;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Misc.WorldMap.WorldBuilding
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class GameEntitySizeMapInitSystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    PlayerBuildingProfilesSOHolder
                    , AfterBakedPrefabsElement >()
                .Build();

            this.RequireForUpdate(this.query);

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
            var profilesSOHolder = this.query.GetSingleton<PlayerBuildingProfilesSOHolder>();
            var afterBakedPrefabsArray = this.query.GetSingletonBuffer<AfterBakedPrefabsElement>().ToNativeArray(Allocator.Temp);
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            var gameEntitySizeMap = new GameEntitySizeMap
            {
                Value = new(20, Allocator.Persistent),
            };

            int tempIndex = 0;

            foreach (var profile in profilesSOHolder.Value.Value.Profiles)
            {
                var keyEntity = afterBakedPrefabsArray[tempIndex].PrimaryEntity;
                if (keyEntity == Entity.Null) continue;

                if (!gameEntitySizeMap.Value.TryAdd(keyEntity, profile.Value.GameEntitySize))
                {
                    UnityEngine.Debug.LogWarning($"{nameof(GameEntitySizeMap)} already contains key: {keyEntity}, which mean more than 2 {nameof(PlayerBuildingProfileElement)} use the same GO as PrimaryEntityPrefab");
                }

                tempIndex++;

            }

            su.AddOrSetComponentData(gameEntitySizeMap);
            afterBakedPrefabsArray.Dispose();

        }

    }

}