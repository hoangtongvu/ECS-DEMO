using Components.GameEntity.Misc;
using Components.Misc.Presenter;
using Unity.Entities;
using UnityEngine;

namespace Systems.Initialization.GameEntity.Misc
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateBefore(typeof(DestroyEntityWithTagSystem))]
    public partial class DestroyBasePresenterWithTagSystem : SystemBase
    {
        private EntityQuery query;

        protected override void OnCreate()
        {
            this.query = SystemAPI.QueryBuilder()
                .WithAll<
                    NeedDestroyBasePresenterTag
                    , PresenterHolder>()
                .Build();

            this.RequireForUpdate(this.query);
        }

        protected override void OnUpdate()
        {
            var presentersTransformAccessArrayGO = SystemAPI.GetSingleton<PresentersTransformAccessArrayGOHolder>().Value.Value;

            foreach (var (presenterHolderRef, entity) in SystemAPI
                .Query<
                    RefRW<PresenterHolder>>()
                .WithAll<
                    NeedDestroyBasePresenterTag>()
                .WithEntityAccess())
            {
                var go = presenterHolderRef.ValueRO.Value.Value.gameObject;
                GameObject.Destroy(go);
                presenterHolderRef.ValueRW.Value = null;

                if (!SystemAPI.HasComponent<TransformAccessArrayIndex>(entity)) continue;
                var transformAccessIndexRef = SystemAPI.GetComponentRW<TransformAccessArrayIndex>(entity);

                presentersTransformAccessArrayGO.RemoveTransformAt(transformAccessIndexRef.ValueRO.Value);
                transformAccessIndexRef.ValueRW = TransformAccessArrayIndex.Invalid;

            }

        }

    }

}