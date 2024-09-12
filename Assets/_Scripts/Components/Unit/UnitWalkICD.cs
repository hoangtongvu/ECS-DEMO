using Unity.Entities;

namespace Components.Unit
{

    public struct NeedsInitWalkTag : IComponentData, IEnableableComponent
    {
    }
    
    public struct WalkSpeed : IComponentData
    {
        public float Value;
    }


}
