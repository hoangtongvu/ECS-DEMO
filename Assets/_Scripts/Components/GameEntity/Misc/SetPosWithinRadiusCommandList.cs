using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Components.GameEntity.Misc
{
    [System.Serializable]
    public struct SetPosWithinRadiusCommand
    {
        public Entity BaseEntity;
        public float3 CenterPos;
        public float Radius;
    }

    public struct SetPosWithinRadiusCommandList : IComponentData
    {
        public NativeList<SetPosWithinRadiusCommand> Value;
    }

}
