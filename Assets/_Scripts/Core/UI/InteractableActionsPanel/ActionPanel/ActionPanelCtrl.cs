using Core.Misc;
using Core.MyEvent.PubSub.Messengers;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
using ZBase.Foundation.PubSub;

namespace Core.UI.InteractableActionsPanel.ActionPanel
{
    [RequireComponent(typeof(TransformScaler))]
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

        public virtual void Activate()
        {
            GameplayMessenger.MessagePublisher
                .Publish<OnAnyActionPanelActivatedMessage>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.actionsContainerUICtrl.ChosenActionPanelCtrl = this;
        }

    }

}