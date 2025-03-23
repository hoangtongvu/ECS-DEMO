using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc.GameView
{
    public struct PlayerViewCamOffset : IComponentData
    {
        public float3 Value;
    }

}
