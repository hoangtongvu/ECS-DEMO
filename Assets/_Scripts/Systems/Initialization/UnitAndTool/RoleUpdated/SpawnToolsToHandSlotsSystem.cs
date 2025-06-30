using Components.GameEntity;
using Components.Misc.Presenter;
using Components.Misc.Presenter.PresenterPrefabGO;
using Components.Unit;
using Components.Unit.Misc;
using Unity.Entities;
using UnityEngine;

namespace Systems.Initialization.UnitAndTool.RoleUpdated
{
    [UpdateInGroup(typeof(RoleUpdatedSystemGroup))]
    [UpdateAfter(typeof(GetHandSlotsSystem))]
    public partial class SpawnToolsToHandSlotsSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    UnitToolHolder
                    , PresenterHandSlotsHolder
                    , NeedRoleUpdatedTag
                    , HasPresenterPrefabGOTag>()
                .Build();

            this.RequireForUpdate(query0);
            this.RequireForUpdate<OriginalPresenterGOMap>();

        }

        protected override void OnUpdate()
        {
            var originalPresenterGOMap = SystemAPI.GetSingleton<OriginalPresenterGOMap>().Value;

            foreach (var (unitToolHolderRef, presenterHandSlotsHolderRef) in
                SystemAPI.Query<
                    RefRO<UnitToolHolder>
                    , RefRO<PresenterHandSlotsHolder>>()
                    .WithAll<
                        NeedRoleUpdatedTag
                        , HasPresenterPrefabGOTag>())
            {
                var rightHandMarker = presenterHandSlotsHolderRef.ValueRO.RightHand.Value;

                var toolEntity = unitToolHolderRef.ValueRO.Value;
                var toolPrimaryPrefab = SystemAPI.GetComponent<PrimaryPrefabEntityHolder>(toolEntity);

                var toolPresenterGOPrefab = originalPresenterGOMap[toolPrimaryPrefab].Value;

                GameObject.Instantiate(toolPresenterGOPrefab, rightHandMarker.transform, false);

            }

        }

    }

}