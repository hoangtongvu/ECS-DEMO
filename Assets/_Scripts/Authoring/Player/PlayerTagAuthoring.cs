using Authoring.Utilities.Extensions;
using Components.GameEntity.Attack;
using Components.GameEntity.Damage;
using Components.GameEntity.InteractableActions;
using Components.GameEntity.Interaction;
using Components.GameEntity.Misc;
using Components.GameEntity.Movement;
using Components.GameEntity.Reaction;
using Components.Misc;
using Components.Misc.Presenter;
using Components.Player;
using Components.Unit.NearUnitDropItems;
using Core.GameEntity.Misc;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Player
{
    public class PlayerTagAuthoring : MonoBehaviour
    {
        private class Baker : Baker<PlayerTagAuthoring>
        {
            public override void Bake(PlayerTagAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<PlayerTag>(entity);
                AddComponent<ItemPickerTag>(entity);

                AddBuffer<NearbyUnitDropItemTimerElement>(entity);

                AddComponent<NeedSpawnPresenterTag>(entity);
                this.AddAndDisableComponent<NewlyDeadTag>(entity);

                AddComponent<InteractableEntityTag>(entity);
                AddComponent(entity, new ArmedStateHolder
                {
                    Value = ArmedState.True,
                });

                AddComponent(entity, new FactionIndex { Value = 1 });

                AddComponent(entity, new DmgValue
                {
                    Value = 10,
                });

                AddComponent<NearestInteractableEntity>(entity);

                AddComponent(entity, new PlayerInteractRadius
                {
                    Value = new(3f),
                });

                AddComponent(entity, LookDirectionXZ.DefaultValue);
                AddComponent<InAttackStateTimeStamp>(entity);
                this.AddAndDisableComponent<InAttackStateTag>(entity);

                AddComponent<MoveableEntityTag>(entity);
                this.AddAndDisableComponent<CanMoveEntityTag>(entity);

                AddComponent<MoveDirectionFloat2>(entity);
                AddComponent<CurrentMoveSpeed>(entity);
                AddComponent<TargetMoveSpeed>(entity);

                this.AddAndDisableComponent<IdleReaction.StartedTag>(entity);
                this.AddAndDisableComponent<IdleReaction.CanUpdateTag>(entity);
                this.AddAndDisableComponent<IdleReaction.UpdatingTag>(entity);
                this.AddAndDisableComponent<IdleReaction.EndedTag>(entity);

                this.AddAndDisableComponent<WalkReaction.StartedTag>(entity);
                this.AddAndDisableComponent<WalkReaction.CanUpdateTag>(entity);
                this.AddAndDisableComponent<WalkReaction.UpdatingTag>(entity);
                this.AddAndDisableComponent<WalkReaction.EndedTag>(entity);

                this.AddAndDisableComponent<RunReaction.StartedTag>(entity);
                this.AddAndDisableComponent<RunReaction.CanUpdateTag>(entity);
                this.AddAndDisableComponent<RunReaction.UpdatingTag>(entity);
                this.AddAndDisableComponent<RunReaction.EndedTag>(entity);

                this.AddAndDisableComponent<AttackReaction.StartedTag>(entity);
                this.AddAndDisableComponent<AttackReaction.CanUpdateTag>(entity);
                this.AddAndDisableComponent<AttackReaction.UpdatingTag>(entity);
                this.AddAndDisableComponent<AttackReaction.EndedTag>(entity);
                AddComponent<AttackReaction.TimerSeconds>(entity);

                // Note: These are magic numbers taken from the attack animation
                AddComponent(entity, new AttackDurationSeconds
                {
                    Value = new(1.1f),
                });

                this.AddAndDisableComponent<AttackEventTriggeredTag>(entity);
                AddComponent(entity, new AttackEventTimestamp
                {
                    Value = new(0.5f),
                });

            }

        }

    }

}
