using Components.GameEntity;
using Components.Misc.Presenter;
using Components.Misc.Presenter.PresenterPrefabGO;
using Components.Unit;
using Components.Unit.Misc;
using Core.Animator;
using Core.Misc.Presenter;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine.SceneManagement;

namespace Systems.Initialization.UnitAndTool.RoleUpdated
{
    [UpdateInGroup(typeof(RoleUpdatedSystemGroup))]
    [UpdateAfter(typeof(PrimaryEntityUpdateOnRoleUpdateSystem))]
    public partial class PresenterGOUpdateOnRoleUpdateSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitProfileIdHolder
                    , PrimaryPrefabEntityHolder
                    , NeedRoleUpdatedTag>()
                .Build();

            this.RequireForUpdate(query0);
            this.RequireForUpdate<PresenterPrefabGOMap>();
            this.RequireForUpdate<PresentersTransformAccessArrayGOHolder>();
        }

        protected override void OnUpdate()
        {
            var presenterPrefabGOMap = SystemAPI.GetSingleton<PresenterPrefabGOMap>().Value;
            var presentersScene = SystemAPI.GetSingleton<PresentersHolderScene>();
            var presentersTransformAccessArrayGO = SystemAPI.GetSingleton<PresentersTransformAccessArrayGOHolder>().Value.Value;

            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (primaryPrefabEntityHolderRef, transformRef, transformAccessArrayIndexRef, presenterHolderRef, entity) in
                SystemAPI.Query<
                    RefRO<PrimaryPrefabEntityHolder>
                    , RefRO<LocalTransform>
                    , RefRO<TransformAccessArrayIndex>
                    , RefRW<PresenterHolder>>()
                    .WithAll<NeedRoleUpdatedTag>()
                    .WithEntityAccess())
            {
                var prefabToSpawn = presenterPrefabGOMap[primaryPrefabEntityHolderRef.ValueRO].Value;

                var newPresenter = BasePresenterPoolMap.Instance.Rent(prefabToSpawn.gameObject);
                newPresenter.transform.SetPositionAndRotation(
                    transformRef.ValueRO.Position
                    , transformRef.ValueRO.Rotation);

                presentersTransformAccessArrayGO.TransformAccessArray[transformAccessArrayIndexRef.ValueRO.Value] =
                    newPresenter.transform;

                newPresenter.gameObject.SetActive(true);

                var oldPresenter = presenterHolderRef.ValueRO.Value.Value;
                BasePresenterPoolMap.Instance.Return(oldPresenter);

                presenterHolderRef.ValueRW.Value = newPresenter;

                this.TrySetAnimatorHolder(ecb, entity, newPresenter);
                SceneManager.MoveGameObjectToScene(newPresenter.gameObject, presentersScene.Value);

            }

            ecb.Playback(this.EntityManager);

        }

        private void TrySetAnimatorHolder(EntityCommandBuffer ecb, in Entity entity, BasePresenter newPresenter)
        {
            if (!newPresenter.TryGetBaseAnimator(out var animator))
            {
                if (SystemAPI.HasComponent<AnimatorHolder>(entity))
                    ecb.RemoveComponent<AnimatorHolder>(entity);

                throw new System.NullReferenceException($"Presenter with name {newPresenter.gameObject.name} is expected to have {nameof(BaseAnimator)}, but it's is missing.");
            }

            ecb.AddComponent(entity, new AnimatorHolder
            {
                Value = animator,
            });

        }

    }

}