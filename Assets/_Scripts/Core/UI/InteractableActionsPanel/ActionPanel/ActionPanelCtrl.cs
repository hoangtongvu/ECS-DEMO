using Unity.Entities;
using UnityEngine;

namespace Core.UI.InteractableActionsPanel.ActionPanel
{
    public abstract class ActionPanelCtrl : BaseUICtrl
    {
        [field:SerializeField]
        public Entity BaseEntity { get; private set; }

        [field: SerializeField]
        public sbyte PriorityInContainer { get; private set; }

        public virtual void Initialize(in Entity baseEntity, sbyte priority)
        {
            this.BaseEntity = baseEntity;
            this.PriorityInContainer = priority;
        }

    }

}