using Components;
using Components.Misc.Presenter;
using Core.Misc.Presenter;
using Unity.Entities;

namespace Systems.Presentation.Misc.Presenter
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class AnimationSyncSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    NeedSpawnPresenterTag
                    , AnimatorData
                    , PresenterHolder>()
                .Build();

            this.RequireForUpdate(query0);
            this.RequireForUpdate<PresentersHolderScene>();

        }

        protected override void OnUpdate()
        {
            foreach (var (animDataRef, animatorTransitionDurationRef, presenterRef) in
                SystemAPI.Query<
                    RefRW<AnimatorData>
                    , RefRO<AnimatorTransitionDuration>
                    , RefRO<PresenterHolder>>()
                    .WithDisabled<NeedSpawnPresenterTag>())
            {
                if (!animDataRef.ValueRO.Value.ValueChanged) continue;

                BasePresenter presenter = presenterRef.ValueRO.Value.Value;
                presenter.BaseAnimator.WaitPlay(animDataRef.ValueRO.Value.Value.ToString(), animatorTransitionDurationRef.ValueRO.Value);
                animDataRef.ValueRW.Value.ValueChanged = false;

            }

        }

    }

}