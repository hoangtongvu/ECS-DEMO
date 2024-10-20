using Components.Misc.Presenter;
using Core.Misc.Presenter;
using Unity.Entities;
using Unity.Transforms;

namespace Systems.Presentation.Misc.Presenter
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class TransformSyncSystem : SystemBase
    {

        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    NeedSpawnPresenterTag
                    , LocalTransform
                    , PresenterHolder>()
                .Build();

            this.RequireForUpdate(query0);
            this.RequireForUpdate<PresentersHolderGO>();
        }

        protected override void OnUpdate()
        {
            var presentersHolder = SystemAPI.GetSingleton<PresentersHolderGO>();

            if (!presentersHolder.Value.Value.gameObject.activeSelf) return;

            foreach (var (transformRef, presenterRef) in
                SystemAPI.Query<
                    RefRO<LocalTransform>
                    , RefRO<PresenterHolder>>()
                    .WithDisabled<NeedSpawnPresenterTag>())
            {
                BasePresenter presenter = presenterRef.ValueRO.Value.Value;
                presenter.transform.SetPositionAndRotation(transformRef.ValueRO.Position, transformRef.ValueRO.Rotation);
            }

        }


    }
}