using Core.UI.InteractableActionsPanel;
using Unity.Entities;

namespace Components.Unit.InteractableActions
{
    public struct ActionsContainerUIHolder : IComponentData
    {
        public UnityObjectRef<ActionsContainerUICtrl> Value;
    }

}
