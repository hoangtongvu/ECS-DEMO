using Core.UI.Identification;
using Unity.Entities;
using Unity.Mathematics;

namespace Components
{

    public struct UISpawned : IComponentData
    {
        public float3 SpawnPosOffset;
        public bool IsSpawned;
        public UIType UIType;
    }

}
