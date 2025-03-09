using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc.GameView
{
    public struct GameViewFixedAngleMap : IComponentData
    {
        public NativeArray<float3> Value;
    }

}
