using Core.Player.Reaction;
using Unity.Entities;

namespace Components.Player.Misc
{
    public struct PlayerReactionConfigsHolder : ISharedComponentData
    {
        public PlayerReactionConfigs Value;
    }
}
