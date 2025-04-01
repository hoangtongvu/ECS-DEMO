using Components;
using Components.GameEntity;
using Components.GameResource;
using Core.GameResource;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.GameResource
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class ResourceItemPresenterEntityMapInitSystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    ResourceProfilesSOHolder
                    , AfterBakedPrefabsElement>()
                .Build();

            this.RequireForUpdate(this.query);
            this.RequireForUpdate<EnumLength<ResourceType>>();

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
            var profilesSOHolder = this.query.GetSingleton<ResourceProfilesSOHolder>();
            var afterBakedPrefabsArray = this.query.GetSingletonBuffer<AfterBakedPrefabsElement>().ToNativeArray(Allocator.Temp);
            int resourceCount = SystemAPI.GetSingleton<EnumLength<ResourceType>>().Value;
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            var resourceItemPresenterEntityPrefabMap = new ResourceItemPresenterEntityPrefabMap
            {
                Value = new(resourceCount, Allocator.Persistent),
            };

            int tempIndex = 0;

            foreach (var profile in profilesSOHolder.Value.Value.Profiles)
            {
                var presenterEntity = afterBakedPrefabsArray[tempIndex].PresenterEntity;
                if (presenterEntity == Entity.Null) continue;

                resourceItemPresenterEntityPrefabMap.Value.Add(profile.Key, presenterEntity);

                tempIndex++;
            }

            su.AddOrSetComponentData(resourceItemPresenterEntityPrefabMap);
            afterBakedPrefabsArray.Dispose();

        }

    }

}