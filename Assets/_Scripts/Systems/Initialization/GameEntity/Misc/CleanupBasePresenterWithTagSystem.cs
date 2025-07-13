using Components.GameEntity.Misc;
using Components.GameEntity.Misc.EntityCleanup;
using Components.Misc.Presenter;
using Systems.Initialization.GameEntity.Misc.EntityCleanup;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Systems.Initialization.GameEntity.Misc
{
    [UpdateInGroup(typeof(CleanupEntityHandleSystemGroup))]
    public partial class CleanupBasePresenterWithTagSystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    NeedDestroyBasePresenterTag
                    , NeedCleanupEntityTag
                    , PresenterHolder>()
                .Build();

            this.RequireForUpdate(this.query);
        }

        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (presenterHolderRef, entity) in SystemAPI
                .Query<
                    RefRO<PresenterHolder>>()
                .WithAll<
                    NeedDestroyBasePresenterTag
                    , NeedCleanupEntityTag>()
                .WithEntityAccess())
            {
                var go = presenterHolderRef.ValueRO.Value.Value.gameObject;
                GameObject.Destroy(go);

                ecb.RemoveComponent<NeedDestroyBasePresenterTag>(entity);
                ecb.RemoveComponent<PresenterHolder>(entity);
            }

            ecb.Playback(this.EntityManager);

        }

    }

}