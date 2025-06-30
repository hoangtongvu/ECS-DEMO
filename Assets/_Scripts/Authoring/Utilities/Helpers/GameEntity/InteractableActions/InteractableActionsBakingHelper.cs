using Authoring.Utilities.Extensions;
using Components.GameEntity.InteractableActions;
using Unity.Entities;

namespace Authoring.Utilities.Helpers.GameEntity.InteractableActions
{
    public static class InteractableActionsBakingHelper
    {
        public static void AddComponents(IBaker baker, in Entity entity)
        {
            baker.AddComponent<ActionsContainerUIHolder>(entity);
            baker.AddAndDisableComponent<CanShowActionsContainerUITag>(entity);
            baker.AddAndDisableComponent<ActionsContainerUIShownTag>(entity);
            baker.AddAndDisableComponent<NewlyActionTriggeredTag>(entity);

        }

    }

}
