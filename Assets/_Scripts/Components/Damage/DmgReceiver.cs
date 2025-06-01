using Unity.Entities;

namespace Components.Damage
{
    public struct HpComponent : IComponentData
    {
        public int CurrentHp;
        public int MaxHp;
    }

    public struct HpChangedTag : IComponentData, IEnableableComponent
    {
    }

    // NOTE: positive number -> healing, negative number -> taking dmg.
    public struct HpChangedValue : IComponentData
    {
        public int Value;
    }

    public struct IsAliveTag : IComponentData, IEnableableComponent
    {
    }

}
