using Components.Misc.Presenter;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using Utilities;

namespace Systems.Initialization.Misc.Presenter
{


    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class SpawnPresenterSystem : SystemBase
    {

        protected override void OnCreate()
        {
            this.CreatePresentersHolder();

            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    NeedSpawnPresenterTag
                    , LocalTransform
                    , PresenterPrefabIdHolder
                    , PresenterHolder>()
                .Build();

            this.RequireForUpdate(query0);
        }

        protected override void OnUpdate()
        {
            var presenterPrefabMap = SystemAPI.GetSingleton<PresenterPrefabMap>();
            var presentersHolder = SystemAPI.GetSingleton<PresentersHolderGO>();

            foreach (var (needSpawnPresenterTag, transformRef, presenterIdRef, presenterRef) in
                SystemAPI.Query<
                    EnabledRefRW<NeedSpawnPresenterTag>
                    , RefRO<LocalTransform>
                    , RefRO<PresenterPrefabIdHolder>
                    , RefRW<PresenterHolder>>())
            {

                if (!presenterPrefabMap.Value.TryGetValue(presenterIdRef.ValueRO.Value, out var presenterPrefab))
                {
                    Debug.LogError($"Can't find any presenter prefab with id {presenterIdRef.ValueRO.Value}");
                    continue;
                }

                var newPresenter = GameObject.Instantiate(
                    presenterPrefab.Value
                    , transformRef.ValueRO.Position
                    , transformRef.ValueRO.Rotation
                    , presentersHolder.Value);

                presenterRef.ValueRW.Value = newPresenter;

                needSpawnPresenterTag.ValueRW = false;

            }

        }

        private void CreatePresentersHolder()
        {
            var holderGO = new PresentersHolderGO
            {
                Value = new GameObject("PresentersHolder").transform,
            };

            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(holderGO);

        }

    }

}