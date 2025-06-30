using Core.UI.InteractableActionsPanel;
using Unity.Entities;

namespace Components.GameEntity.InteractableActions
{
    public struct ActionsContainerUIHolder : IComponentData
    {
        public UnityObjectRef<ActionsContainerUICtrl> Value;
    }

}
