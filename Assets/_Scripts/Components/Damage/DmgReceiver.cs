using Unity.Entities;

namespace Components.Damage
{
    public struct HpComponent : IComponentData
    {
        public int CurrentHp;
        public int MaxHp;
    }

    public struct HpChangeState : IComponentData // TODO: Find a way to store Hp change history (using queue?) (system dependency related problem?).
    {
        public bool IsChanged;
        public int ChangedValue; // positive number -> healing, negative number -> taking dmg.
    }

    public struct AliveState : IComponentData
    {
        public bool Value;
    }

}
