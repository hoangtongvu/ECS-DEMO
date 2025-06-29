using Unity.Entities;

namespace Components.Misc.WorldMap.WorldBuilding.BuildingConstruction
{
    public struct BlueprintMaterialHolder : IComponentData
    {
        public UnityObjectRef<UnityEngine.Material> Value;
    }

}
