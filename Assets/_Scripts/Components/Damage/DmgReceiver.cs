using Unity.Entities;

namespace Components.Damage
{
    public struct HpComponent : IComponentData
    {
        public int CurrentHp;
        public int MaxHp;
    }

    // TODO: Find a way to store Hp change history (using queue?) (system dependency related problem?).
    public struct HpChangedTag : IComponentData, IEnableableComponent
    {
    }

    public struct HpChangedValue : IComponentData
    {
        public int Value; //positive number -> healing, negative number -> taking dmg.
    }

    public struct IsAliveTag : IComponentData, IEnableableComponent
    {
    }


}
