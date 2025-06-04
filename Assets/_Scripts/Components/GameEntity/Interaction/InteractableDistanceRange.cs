using Unity.Entities;
using Unity.Mathematics;

namespace Components.GameEntity.Interaction
{
    public struct InteractableDistanceRange : IComponentData
    {
        public half MinValue;
        public half MaxValue;

        public static readonly InteractableDistanceRange Default = new()
        {
            MinValue = new half(0),
            // BUG: MaxValue smaller than cellRadius or cellRadius * 2 might cause bug:
            // StopMoveReachMinDis first, SetCanInteractFlag won't be executed -> No interaction.
            MaxValue = new half(2.0f),
        };
    }

}
