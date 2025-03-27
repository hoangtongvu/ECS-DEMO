using Components.Misc.Presenter;
using Core.Animator;
using Core.Misc.Presenter;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Systems.Initialization.Misc.Presenter
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class SpawnPresenterSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var su = SingletonUtilities.GetInstance(this.EntityManager);

            this.CreatePresentersHolderScene(su, out var newScene);
            this.CreatePresentersTransformAccessArrayGOHolder(su, in newScene);

            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    NeedSpawnPresenterTag
                    , LocalTransform
                    , PresenterPrefabIdHolder
                    , PresenterHolder
                    , TransformAccessArrayIndex>()
                .Build();

            this.RequireForUpdate(query0);

        }

        protected override void OnUpdate()
        {
            var presenterPrefabMap = SystemAPI.GetSingleton<PresenterPrefabMap>();
            var presentersScene = SystemAPI.GetSingleton<PresentersHolderScene>();
            var presentersTransformAccessArrayGOHolder = SystemAPI.GetSingleton<PresentersTransformAccessArrayGOHolder>();

            foreach (var (needSpawnPresenterTag, transformRef, presenterIdRef, presenterRef, transformAccessArrayIndexRef, entity) in
                SystemAPI.Query<
                    EnabledRefRW<NeedSpawnPresenterTag>
                    , RefRO<LocalTransform>
                    , RefRO<PresenterPrefabIdHolder>
                    , RefRW<PresenterHolder>
                    , RefRW<TransformAccessArrayIndex>>()
                    .WithEntityAccess())
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

                SceneManager.MoveGameObjectToScene(newPresenter.gameObject, presentersScene.Value);

                this.TryInitAnimatorHolder(in entity, in newPresenter);

                presentersTransformAccessArrayGOHolder.Value.Value.TransformAccessArray.Add(newPresenter.transform);
                transformAccessArrayIndexRef.ValueRW.Value = presentersTransformAccessArrayGOHolder.Value.Value.TransformAccessArray.length - 1; 

                needSpawnPresenterTag.ValueRW = false;

            }

        }

        private void CreatePresentersHolderScene(SingletonUtilities su, out Scene newScene)
        {
            newScene = SceneManager.CreateScene("PresentersScene");

            su.AddOrSetComponentData(new PresentersHolderScene
            {
                Value = newScene,
            });

        }

        private void CreatePresentersTransformAccessArrayGOHolder(SingletonUtilities su, in Scene presentersHolderScene)
        {
            var newGO = new GameObject("*PresentersTransformAccessArrayGO");
            
            su.AddOrSetComponentData(new PresentersTransformAccessArrayGOHolder
            {
                Value = newGO.AddComponent<PresentersTransformAccessArrayGO>(),
            });

            SceneManager.MoveGameObjectToScene(newGO, presentersHolderScene);
        
        }

        private void TryInitAnimatorHolder(in Entity entity, in BasePresenter newPresenter)
        {
            if (!SystemAPI.HasComponent<AnimatorHolder>(entity)) return;

            if (!newPresenter.TryGetBaseAnimator(out var animator))
                throw new System.NullReferenceException($"Presenter with name {newPresenter.gameObject.name} is expected to have {nameof(BaseAnimator)}, but it's is missing.");

            SystemAPI.GetComponentRW<AnimatorHolder>(entity).ValueRW.Value = animator;

        }

    }

}