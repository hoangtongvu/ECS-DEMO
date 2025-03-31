using Core.Player;
using Unity.Entities;

namespace Components.Player
{
    public struct PlayerProfilesSOHolder : IComponentData
    {
        public UnityObjectRef<PlayerProfilesSO> Value;
    }

}
