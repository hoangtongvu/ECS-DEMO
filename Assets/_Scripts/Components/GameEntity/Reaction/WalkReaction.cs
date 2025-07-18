using Unity.Entities;

namespace Components.GameEntity.Reaction
{
    public struct WalkReaction
    {
        public struct StartedTag : IComponentData, IEnableableComponent
        {
        }

        public struct CanUpdateTag : IComponentData, IEnableableComponent
        {
        }

        public struct UpdatingTag : IComponentData, IEnableableComponent
        {
        }

        public struct EndedTag : IComponentData, IEnableableComponent
        {
        }

        public struct TimerSeconds : IComponentData
        {
            public float Value;
        }

    }

}
