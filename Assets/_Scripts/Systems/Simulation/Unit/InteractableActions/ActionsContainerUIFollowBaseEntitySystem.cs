using Components.GameEntity.Movement;
using Components.Unit.InteractableActions;
using Core.Utilities.Extensions;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Simulation.Unit.InteractableActions
{
    [UpdateInGroup(typeof(ActionUIsHandleSystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(ActionsContainerUIShownTagHandleSystem))]
    public partial class ActionsContainerUIFollowBaseEntitySystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query0 = SystemAPI.QueryBuilder()
                .WithAll<
                    LocalTransform
                    , ActionsContainerUIHolder
                    , ActionsContainerUIShownTag
                    , CanMoveEntityTag>()
                .Build();

            this.RequireForUpdate(query0);
            this.RequireForUpdate<ActionsContainerUIOffsetY>();
        }

        protected override void OnUpdate()
        {
            half offsetY = SystemAPI.GetSingleton<ActionsContainerUIOffsetY>().Value;

            foreach (var (unitTransform, actionsContainerUIHolderRef) in SystemAPI
                .Query<
                    RefRO<LocalTransform>
                    , RefRW<ActionsContainerUIHolder>>()
                .WithAll<
                    ActionsContainerUIShownTag
                    , CanMoveEntityTag>())
            {
                var uiTransform = actionsContainerUIHolderRef.ValueRO.Value.Value.transform;
                uiTransform.position = unitTransform.ValueRO.Position.Add(y: offsetY);
            }

        }

    }

}