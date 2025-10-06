using Authoring.Utilities.Extensions;
using Components.GameEntity.Attack;
using Components.GameEntity.Damage;
using Components.GameEntity.Interaction;
using Components.GameEntity.Interaction.Common;
using Components.GameEntity.Interaction.InteractionPhases;
using Components.GameEntity.Misc;
using Components.GameEntity.Movement;
using Components.GameEntity.Reaction;
using Components.GameResource.ItemPicking;
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
                AddComponent<RotationFreezer>(entity);

                AddBuffer<NearbyUnitDropItemTimerElement>(entity);

                AddComponent<NeedSpawnPresenterTag>(entity);
                this.AddAndDisableComponent<TakeHitEvent>(entity);
                this.AddAndDisableComponent<DeadEvent>(entity);

                AddComponent<InteractableEntityTag>(entity);
                AddComponent(entity, new ArmedStateHolder
                {
                    Value = ArmedState.True,
                });

                AddComponent(entity, new FactionIndex { Value = 1 });

                AddComponent(entity, new BaseDmg
                {
                    Value = 18,
                });

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

                IdleReaction.BakingHelper.BakeTags(this, in entity);

                WalkReaction.BakingHelper.BakeTags(this, in entity);

                RunReaction.BakingHelper.BakeTags(this, in entity);

                AttackReaction.BakingHelper.BakeTags(this, in entity);
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

                AddComponent(entity, MoveSpeedScale.DefaultValue);

                AddBuffer<CandidateItemDistanceHit>(entity);
                this.AddAndDisableComponent<CandidateItemDistanceHitBufferUpdated>(entity);
                AddBuffer<ItemCanBePickedUpIndex>(entity);

                this.AddAndDisableComponent<PreInteractionPhase.StartedEvent>(entity);
                this.AddAndDisableComponent<PreInteractionPhase.CanUpdate>(entity);
                this.AddAndDisableComponent<PreInteractionPhase.Updating>(entity);
                this.AddAndDisableComponent<PreInteractionPhase.EndedEvent>(entity);
                this.AddAndDisableComponent<PreInteractionPhase.CanCancel>(entity);
                this.AddAndDisableComponent<PreInteractionPhase.CanceledEvent>(entity);

                this.AddAndDisableComponent<InteractingPhase.StartedEvent>(entity);
                this.AddAndDisableComponent<InteractingPhase.CanUpdate>(entity);
                this.AddAndDisableComponent<InteractingPhase.Updating>(entity);
                this.AddAndDisableComponent<InteractingPhase.EndedEvent>(entity);
                this.AddAndDisableComponent<InteractingPhase.CanCancel>(entity);
                this.AddAndDisableComponent<InteractingPhase.CanceledEvent>(entity);

                this.AddAndDisableComponent<PostInteractionPhase.StartedEvent>(entity);
                this.AddAndDisableComponent<PostInteractionPhase.CanUpdate>(entity);
                this.AddAndDisableComponent<PostInteractionPhase.Updating>(entity);
                this.AddAndDisableComponent<PostInteractionPhase.EndedEvent>(entity);
                this.AddAndDisableComponent<PostInteractionPhase.CanCancel>(entity);
                this.AddAndDisableComponent<PostInteractionPhase.CanceledEvent>(entity);

            }

        }

    }

}
