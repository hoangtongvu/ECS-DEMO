using Components.GameEntity;
using Components.GameEntity.Movement;
using Components.Misc.Presenter;
using Components.Misc.Presenter.PresenterPrefabGO;
using Core.Misc.Presenter;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

namespace Systems.Initialization.Misc.Presenter.PresenterPrefabGO
{
    [UpdateInGroup(typeof(NeedSpawnPresenterTagProcessSystemGroup))]
    public partial class SpawnPresenterGOsSystem : SystemBase
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
                    , PrimaryPrefabEntityHolder
                    , HasPresenterPrefabGOTag>()
                .Build();

            this.RequireForUpdate(query0);
            this.RequireForUpdate<PresenterPrefabGOMap>();

        }

        protected override void OnUpdate()
        {
            var presenterPrefabGOMap = SystemAPI.GetSingleton<PresenterPrefabGOMap>();
            var presentersScene = SystemAPI.GetSingleton<PresentersHolderScene>();
            var presentersTransformAccessArrayGOHolder = SystemAPI.GetSingleton<PresentersTransformAccessArrayGOHolder>();

            // NOTE: Be careful when using EcbSystem.ecb in this System,
            // TransformAccessArrayIndex is assigned long after TransformAccessArray adding new element.
            // Make sure any system that use TransformAccessArray will get final version of TransformAccessArrayIndexes.
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (transformRef, primaryPrefabEntityHolderRef, entity) in
                SystemAPI.Query<
                    RefRO<LocalTransform>
                    , RefRO<PrimaryPrefabEntityHolder>>()
                    .WithAll<
                        NeedSpawnPresenterTag
                        , HasPresenterPrefabGOTag>()
                    .WithEntityAccess())
            {
                if (!presenterPrefabGOMap.Value.TryGetValue(primaryPrefabEntityHolderRef.ValueRO, out var basePresenterPrefab))
                {
                    UnityEngine.Debug.LogWarning($"Can't find any presenter prefab with Key: {primaryPrefabEntityHolderRef.ValueRO}");
                    continue;
                }

                var newPresenter = GameObject.Instantiate(
                    basePresenterPrefab.Value
                    , transformRef.ValueRO.Position
                    , transformRef.ValueRO.Rotation);

                ecb.AddComponent(entity, new PresenterHolder
                {
                    Value = newPresenter,
                });

                if (SystemAPI.HasComponent<CanMoveEntityTag>(entity))
                {
                    presentersTransformAccessArrayGOHolder.Value.Value.AddTransform(newPresenter.transform, out var newIndex);

                    ecb.AddComponent(entity, new TransformAccessArrayIndex
                    {
                        Value = newIndex,
                    });
                }
                
                this.TryInitAnimatorHolder(ecb, in entity, in newPresenter);

                SceneManager.MoveGameObjectToScene(newPresenter.gameObject, presentersScene.Value);

            }

            ecb.Playback(this.EntityManager);

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

        private void TryInitAnimatorHolder(EntityCommandBuffer ecb, in Entity entity, in BasePresenter newPresenter)
        {
            if (!newPresenter.TryGetBaseAnimator(out var animator)) return;

            ecb.AddComponent(entity, new AnimatorHolder
            {
                Value = animator,
            });

        }

    }

}