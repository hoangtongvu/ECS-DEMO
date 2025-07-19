using Core.Player;
using Unity.Entities;

namespace Components.Player.Misc
{
    public struct PlayerProfileIdHolder : IComponentData
    {
        public PlayerProfileId Value;
    }
}
