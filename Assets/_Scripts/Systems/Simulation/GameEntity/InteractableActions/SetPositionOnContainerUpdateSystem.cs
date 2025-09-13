using Components.GameEntity.InteractableActions;
using Core.Utilities.Extensions;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Simulation.GameEntity.InteractableActions
{
    [UpdateInGroup(typeof(ActionsContainerUpdateSystemGroup))]
    public partial class SetPositionOnContainerUpdateSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    ActionsContainerUI_CD.Holder
                    , ActionsContainerUI_CD.CanUpdate>()
                .Build();

            this.RequireForUpdate(query0);
        }

        protected override void OnUpdate()
        {
            if (!this.CanActionsContainerUpdate()) return;

            var actionsContainerUICtrl = SystemAPI.GetSingleton<ActionsContainerUI_CD.Holder>().Value.Value;
            var nearestInteractableEntity = SystemAPI.GetSingleton<NearestInteractableEntity>().Value;
            half offsetY = SystemAPI.GetSingleton<ActionsContainerUIOffsetY>().Value;

            var targetTransform = SystemAPI.GetComponent<LocalTransform>(nearestInteractableEntity);
            actionsContainerUICtrl.transform.position = targetTransform.Position.Add(y: offsetY);
        }

        private bool CanActionsContainerUpdate()
        {
            foreach (var canUpdateTag in SystemAPI
                .Query<EnabledRefRO<ActionsContainerUI_CD.CanUpdate>>())
            {
                return true;
            }

            return false;
        }

    }

}