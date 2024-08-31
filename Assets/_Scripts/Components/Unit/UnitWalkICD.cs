using Unity.Entities;

namespace Components.Unit
{
    public struct CanUnitWalkTag : IComponentData, IEnableableComponent
    {
    }
    
    public struct NeedInitWalkTag : IComponentData, IEnableableComponent
    {
    }

    public struct WalkSpeed : IComponentData
    {
        public float Value;
    }

    public struct WalkTime : IComponentData
    {
        public float TimeCounterSecond;
        public float TimeDurationSecond;
    }


}
