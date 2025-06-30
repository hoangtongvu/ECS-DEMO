using Components.Misc.Presenter;
using Components.Misc.Presenter.PresenterPrefabGO;
using Components.Unit.Misc;
using Core.Misc.Presenter;
using Systems.Initialization.UnitAndTool.RoleUpdated;
using Unity.Entities;

namespace Systems.Initialization.Unit.Misc
{
    [UpdateInGroup(typeof(RoleUpdatedSystemGroup))]
    [UpdateAfter(typeof(PresenterGOUpdateOnRoleUpdateSystem))]
    public partial class GetHandSlotsSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    PresenterHolder
                    , PresenterHandSlotsHolder
                    , NeedRoleUpdatedTag
                    , HasPresenterPrefabGOTag>()
                .Build();

            this.RequireForUpdate(query0);

        }

        protected override void OnUpdate()
        {
            foreach (var (presenterHolderRef, presenterHandSlotsHolderRef) in
                SystemAPI.Query<
                    RefRO<PresenterHolder>
                    , RefRW<PresenterHandSlotsHolder>>()
                    .WithAll<
                        NeedRoleUpdatedTag
                        , HasPresenterPrefabGOTag>())
            {
                var basePresenter = presenterHolderRef.ValueRO.Value.Value;
                var handSlotMarkers = basePresenter.GetComponentsInChildren<HandSlotMarker>();

                int length = handSlotMarkers.Length;

                if (length > 2)
                {
                    UnityEngine.Debug.LogWarning($"There are more than 2 hand slots on this {nameof(BasePresenter)}", basePresenter.gameObject);
                    continue;
                }

                for (int i = 0; i < length; i++)
                {
                    var handSlotMarker = handSlotMarkers[i];

                    switch (handSlotMarker.HandSlotSide)
                    {
                        case HandSlotSide.Right:
                            presenterHandSlotsHolderRef.ValueRW.RightHand = handSlotMarker;
                            break;
                        case HandSlotSide.Left:
                            presenterHandSlotsHolderRef.ValueRW.LeftHand = handSlotMarker;
                            break;
                        default:
                            break;

                    }
                }

            }

        }

    }

}