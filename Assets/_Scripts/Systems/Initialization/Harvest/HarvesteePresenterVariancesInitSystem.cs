using Components.GameEntity;
using Components.Harvest;
using Unity.Collections;
using Unity.Entities;
using Utilities;

namespace Systems.Initialization.Harvest
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class HarvesteePresenterVariancesInitSystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    HarvesteeProfilesSOHolder
                    , BakedGameEntityProfileElement
                    , HarvesteePresenterVarianceTempBufferElement>()
                .Build();

            this.RequireForUpdate(this.query);

        }

        protected override void OnUpdate()
        {
            this.Enabled = false;
            var profilesSOHolder = this.query.GetSingleton<HarvesteeProfilesSOHolder>();
            var presenterVarianceTempArray = this.query.GetSingletonBuffer<HarvesteePresenterVarianceTempBufferElement>().ToNativeArray(Allocator.Temp);
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            var profileIdToPresenterVariancesRangeMap = new HarvesteeProfileIdToPresenterVariancesRangeMap
            {
                Value = new(15, Allocator.Persistent),
            };

            var presenterVariancesContainer = new HarvesteePresenterVariancesContainer
            {
                Value = new(15, Allocator.Persistent),
            };

            int tempIndex = 0;

            foreach (var pair in profilesSOHolder.Value.Value.Profiles)
            {
                int presenterVarianceCount = pair.Value.PresenterVariances.Count;
                int upperBound = tempIndex + presenterVarianceCount;

                profileIdToPresenterVariancesRangeMap.Value.Add(pair.Key, new()
                {
                    StartIndex = tempIndex,
                    Count = presenterVarianceCount,
                });

                //UnityEngine.Debug.Log($"[{pair.Key}] startIndex: {tempIndex}, count: {presenterVarianceCount}");

                for (; tempIndex < upperBound; tempIndex++)
                {
                    presenterVariancesContainer.Value.Add(presenterVarianceTempArray[tempIndex].Value);
                    //UnityEngine.Debug.Log($"[{pair.Key}] element: {presenterVarianceTempArray[tempIndex].Value}");
                }

            }

            presenterVarianceTempArray.Dispose();
            su.AddOrSetComponentData(profileIdToPresenterVariancesRangeMap);
            su.AddOrSetComponentData(presenterVariancesContainer);

            this.EntityManager.RemoveComponent<HarvesteePresenterVarianceTempBufferElement>(this.query.GetSingletonEntity());

        }

    }

}