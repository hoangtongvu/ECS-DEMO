using System.Runtime.InteropServices;
using System;

namespace Core.Unit.MyMoveCommand
{
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct MoveCommandId : IEquatable<MoveCommandId>
    {

        [FieldOffset(0)] private readonly int _raw; // 4 bytes

        [FieldOffset(0)] public readonly UnitType Unit; // 1 byte
        [FieldOffset(1)] public readonly MoveCommand MoveCommand; // 1 byte
        [FieldOffset(2)] public readonly ushort LocalIndex; // 2 bytes

        public MoveCommandId(UnitType unit, MoveCommand moveCommand) : this()
        {
            Unit = unit;
            MoveCommand = moveCommand;
        }

        public MoveCommandId(UnitType unit, MoveCommand moveCommand, ushort localIndex) : this()
        {
            Unit = unit;
            MoveCommand = moveCommand;
            LocalIndex = localIndex;
        }


        public bool Equals(MoveCommandId other)
            => _raw == other._raw;

        public override bool Equals(object obj)
            => obj is MoveCommandId other && _raw == other._raw;

        public override int GetHashCode()
            => _raw.GetHashCode();

        public static bool operator ==(MoveCommandId left, MoveCommandId right)
            => left._raw == right._raw;

        public static bool operator !=(MoveCommandId left, MoveCommandId right)
            => left._raw != right._raw;

        public override string ToString()
        {
            return $"{nameof(Unit)}: {Unit}, {nameof(MoveCommand)}: {MoveCommand}, {nameof(LocalIndex)}: {LocalIndex}";
        }

    }
}