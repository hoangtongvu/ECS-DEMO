using Core.Misc;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Core.UI.InteractableActionsPanel.ActionPanel
{
    public abstract class ActionPanelCtrl : BaseUICtrl, IPointerEnterHandler
    {
        [SerializeField] protected ActionsContainerUICtrl actionsContainerUICtrl;
        [SerializeField] protected TransformScaler transformScaler;

        [field:SerializeField]
        public Entity BaseEntity { get; private set; }

        [field: SerializeField]
        public sbyte PriorityInContainer { get; private set; }

        protected override void LoadComponents()
        {
            base.LoadComponents();
            this.transformScaler = GetComponent<TransformScaler>();
        }

        public virtual void Initialize(in Entity baseEntity, sbyte priority, ActionsContainerUICtrl actionsContainerUICtrl)
        {
            this.BaseEntity = baseEntity;
            this.PriorityInContainer = priority;
            this.actionsContainerUICtrl = actionsContainerUICtrl;
        }

        public virtual void OnBeingChosen() => this.transformScaler.ScaleUp();

        public virtual void OnBeingUnchosen() => this.transformScaler.ScaleDown();

        public abstract void Activate();

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.actionsContainerUICtrl.ChosenActionPanelCtrl = this;
        }

    }

}