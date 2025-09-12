using Components.GameEntity.InteractableActions;
using Components.GameEntity.Movement;
using Core.Utilities.Extensions;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Simulation.GameEntity.InteractableActions
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class ActionsContainerUIFollowBaseEntitySystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    LocalTransform
                    , CanMoveEntityTag>()
                .Build();

            this.RequireForUpdate(query0);
            this.RequireForUpdate<ActionsContainerUI_CD.Holder>();
            this.RequireForUpdate<ActionsContainerUIOffsetY>();
        }

        protected override void OnUpdate()
        {
            var actionsContainerUICtrl = SystemAPI.GetSingleton<ActionsContainerUI_CD.Holder>().Value.Value;
            if (!actionsContainerUICtrl) return;

            half offsetY = SystemAPI.GetSingleton<ActionsContainerUIOffsetY>().Value;

            foreach (var transformRef in SystemAPI
                .Query<
                    RefRO<LocalTransform>>()
                .WithAll<CanMoveEntityTag>()
                .WithAll<IsTargetForActionsContainerUI>())
            {
                var uiTransform = actionsContainerUICtrl.transform;
                uiTransform.position = transformRef.ValueRO.Position.Add(y: offsetY);
            }

        }

    }

}