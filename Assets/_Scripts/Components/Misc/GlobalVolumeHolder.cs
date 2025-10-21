using Unity.Entities;
using UnityEngine.Rendering;

namespace Components.Misc;

public struct GlobalVolumeHolder : IComponentData
{
    public UnityObjectRef<Volume> Value;
}