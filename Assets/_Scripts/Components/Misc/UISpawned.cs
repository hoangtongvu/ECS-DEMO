using Core.UI.Identification;
using Unity.Entities;
using Unity.Mathematics;

namespace Components.Misc
{
    public struct UISpawned : IComponentData
    {
        public UIID? UIID;
        public float3 SpawnPosOffset;
        public bool IsSpawned;
    }

}
