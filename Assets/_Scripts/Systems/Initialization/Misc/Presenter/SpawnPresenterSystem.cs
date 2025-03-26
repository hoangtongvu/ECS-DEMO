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
            this.CreatePresentersHolderScene();

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
            var presentersScene = SystemAPI.GetSingleton<PresentersHolderScene>();

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
                    , transformRef.ValueRO.Rotation);

                presenterRef.ValueRW.Value = newPresenter;

                UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(newPresenter.gameObject, presentersScene.Value);

                needSpawnPresenterTag.ValueRW = false;

            }

        }

        private void CreatePresentersHolderScene()
        {
            SingletonUtilities.GetInstance(this.EntityManager)
                .AddOrSetComponentData(new PresentersHolderScene
                {
                    Value = UnityEngine.SceneManagement.SceneManager.CreateScene("PresentersScene"),
                });

        }

    }

}