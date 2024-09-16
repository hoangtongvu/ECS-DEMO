using System.Runtime.InteropServices;
using System;

namespace Core.Unit.MyMoveCommand
{
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct MoveCommandSourceId : IEquatable<MoveCommandSourceId>
    {

        [FieldOffset(0)] private readonly int _raw; // 4 bytes

        [FieldOffset(0)] public readonly UnitType Unit; // 1 byte
        [FieldOffset(1)] public readonly MoveCommandSource MoveCommandSource; // 1 byte
        [FieldOffset(2)] public readonly ushort LocalIndex; // 2 bytes

        public MoveCommandSourceId(UnitType unit, MoveCommandSource moveCommandSource) : this()
        {
            Unit = unit;
            MoveCommandSource = moveCommandSource;
        }

        public MoveCommandSourceId(UnitType unit, MoveCommandSource moveCommandSource, ushort localIndex) : this()
        {
            Unit = unit;
            MoveCommandSource = moveCommandSource;
            LocalIndex = localIndex;
        }


        public bool Equals(MoveCommandSourceId other)
            => _raw == other._raw;

        public override bool Equals(object obj)
            => obj is MoveCommandSourceId other && _raw == other._raw;

        public override int GetHashCode()
            => _raw.GetHashCode();

        public static bool operator ==(MoveCommandSourceId left, MoveCommandSourceId right)
            => left._raw == right._raw;

        public static bool operator !=(MoveCommandSourceId left, MoveCommandSourceId right)
            => left._raw != right._raw;

        public override string ToString()
        {
            return $"{nameof(Unit)}: {Unit}, {nameof(MoveCommandSource)}: {MoveCommandSource}, {nameof(LocalIndex)}: {LocalIndex}";
        }

    }
}