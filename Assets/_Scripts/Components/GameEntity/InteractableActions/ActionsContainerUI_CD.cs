using Core.UI.InteractableActionsPanel;
using Unity.Entities;

namespace Components.GameEntity.InteractableActions
{
    public struct ActionsContainerUI_CD
    {
        public struct Holder : IComponentData
        {
            public UnityObjectRef<ActionsContainerUICtrl> Value;
        }

        public struct CanShow : IComponentData, IEnableableComponent
        {
        }

        public struct CanUpdate : IComponentData, IEnableableComponent
        {
        }

        public struct IsActive : IComponentData, IEnableableComponent
        {
        }
    }
}
