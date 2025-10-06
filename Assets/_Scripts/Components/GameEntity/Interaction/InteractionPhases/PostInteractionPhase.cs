using Unity.Entities;

namespace Components.GameEntity.Interaction.InteractionPhases;

public struct PostInteractionPhase
{
	public struct StartedEvent : IComponentData, IEnableableComponent
	{
	}

	public struct CanUpdate : IComponentData, IEnableableComponent
    {
	}

	public struct Updating : IComponentData, IEnableableComponent
	{
	}

	public struct EndedEvent : IComponentData, IEnableableComponent
	{
	}

	public struct CanCancel : IComponentData, IEnableableComponent
	{
	}

	public struct CanceledEvent : IComponentData, IEnableableComponent
	{
	}
}
