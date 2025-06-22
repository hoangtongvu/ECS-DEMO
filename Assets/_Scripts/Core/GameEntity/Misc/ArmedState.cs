using EnumLengthGenerator;

namespace Core.GameEntity.Misc
{
    // Note: Make sure this enum elements' underlying values are continuous.
    [GenerateEnumLength]
    public enum ArmedState : byte
    {
        False = 0,
        True = 1,
    }
}