using Components.GameEntity;
using Components.Misc.Presenter.PresenterPrefabGO;
using Components.Unit;
using Core.Misc.Presenter;
using Core.Misc.Presenter.PresenterPrefabGO;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Misc.Presenter.PresenterPrefabGO
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class PresenterPrefabGOMapInitSystem : SystemBase
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
            var afterBakedPrefabsBuffer = this.query.GetSingletonBuffer<AfterBakedPrefabsElement>();
            var em = this.EntityManager;
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            var presenterPrefabGOMap = new PresenterPrefabGOMap()
            {
                Value = new(15, Allocator.Persistent),
            };

            var latestPresenterPrefabGOKey = new LatestPresenterPrefabGOKey
            {
                Value = PresenterPrefabGOKey.DefaultNotNull,
            };

            int tempIndex = 0;

            foreach (var profile in profilesSOHolder.Value.Value.Profiles)
            {
                var targetEntity = afterBakedPrefabsBuffer[tempIndex].PrimaryEntity;
                if (targetEntity == Entity.Null) continue;

                if (profile.Value.PresenterPrefab == null) continue;
                if (!profile.Value.PresenterPrefab.TryGetComponent<BasePresenter>(out var basePresenter)) continue;

                var presenterPrefabGOKey = latestPresenterPrefabGOKey.Value;
                presenterPrefabGOMap.Value.Add(presenterPrefabGOKey, basePresenter);

                em.SetComponentData(targetEntity, new PresenterPrefabGOKeyHolder
                {
                    Value = presenterPrefabGOKey,
                });

                latestPresenterPrefabGOKey.Value.Id++;
                tempIndex++;
            }

            su.AddOrSetComponentData(presenterPrefabGOMap);
            su.AddOrSetComponentData(latestPresenterPrefabGOKey);

        }

    }

}