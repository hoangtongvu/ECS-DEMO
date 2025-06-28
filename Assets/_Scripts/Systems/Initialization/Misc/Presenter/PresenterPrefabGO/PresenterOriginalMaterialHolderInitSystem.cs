using Components.Misc.Presenter;
using Components.Misc.Presenter.PresenterPrefabGO;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Systems.Initialization.Misc.Presenter.PresenterPrefabGO
{
    [UpdateInGroup(typeof(NeedSpawnPresenterTagProcessSystemGroup))]
    [UpdateAfter(typeof(SpawnPresenterGOsSystem))]
    public partial class PresenterOriginalMaterialHolderInitSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    PresenterHolder
                    , NeedSpawnPresenterTag
                    , HasPresenterPrefabGOTag
                    , PresenterOriginalMaterialHolder>()
                .Build();

            this.RequireForUpdate(query0);
        }

        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (presenterHolderRef, entity) in
                SystemAPI.Query<
                    RefRO<PresenterHolder>>()
                    .WithAll<
                        NeedSpawnPresenterTag
                        , HasPresenterPrefabGOTag
                        , PresenterOriginalMaterialHolder>()
                    .WithEntityAccess())
            {
                var basePresenter = presenterHolderRef.ValueRO.Value.Value;
                var originalMat = basePresenter.GetComponent<MeshRenderer>().sharedMaterial;

                ecb.SetSharedComponent(entity, new PresenterOriginalMaterialHolder
                {
                    Value = originalMat,
                });
            }

            ecb.Playback(this.EntityManager);

        }

    }

}