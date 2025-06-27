using Unity.Entities;

namespace Components.Misc.WorldMap.WorldBuilding.Blueprint
{
    public struct BlueprintMaterialHolder : IComponentData
    {
        public UnityObjectRef<UnityEngine.Material> Value;
    }

}
